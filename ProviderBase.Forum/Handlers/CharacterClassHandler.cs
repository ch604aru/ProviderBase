using System;
using ProviderBase.Framework.Handlers;
using ProviderBase.Framework.Forum.Entities;
using System.Collections.Generic;
using ProviderBase.Data.Providers;
using BlizzardAPI.WoW;
using BlizzardAPI.WoW.Entities;
using System.Linq;

namespace ProviderBase.Framework.Forum.Handlers
{
    public class CharacterClassHandler : BaseHandler
    {
        public void GetRecruitmentSummary()
        {
            string formBuilderGUID = ProviderBase.Framework.Utility.GetFormValue<string>("FormBuilder_GUID", "");
            string templateFinal = "";
            
            if (string.IsNullOrEmpty(formBuilderGUID) == false)
            {
                FormBuilderUtility formBuilderUtility = null;
                List<CharacterClass> characterClassList = null;

                formBuilderUtility = new FormBuilderUtility(this.Website, formBuilderGUID);

                characterClassList = ProviderBase.Data.Providers.DataProvider.SelectAll<CharacterClass>(new CharacterClass(), this.Website.WebsiteConnection.ConnectionString);

                templateFinal = formBuilderUtility.Render(characterClassList, this.User);

                this.AjaxResult.Data.Add(templateFinal);
                this.AjaxResult.Message = "Get Recruitment Summary success";
                this.AjaxResult.Status = AjaxResultStatus.Success;
            }
        }

        public void GetUserCharacterSummary()
        {
            string templatePrefix = ProviderBase.Framework.Utility.GetFormValue<string>("TemplatePrefix", "");
            string templateName = "HeaderLoggedInRepeat.htm";
            CharacterUser characterUser = null;

            if (string.IsNullOrEmpty(templatePrefix))
            {
                templatePrefix = this.Website.WebsiteTemplatePrefix;
            }

            if (this.User?.UserID > 0)
            {
                characterUser = DataProvider.SelectSingleFull<CharacterUser>(new CharacterUser()
                {
                    UserID = this.User.UserID
                }, this.Website.WebsiteConnection.ConnectionString);

                if (characterUser?.CharacterUserID > 0)
                {
                    TemplateFile templateFIle = null;

                    templateFIle = new TemplateFile(templatePrefix + templateName, this.Website.WebsiteTemplateFolder);

                    templateFIle.BindTemplateFileContent(characterUser);

                    this.AjaxResult.Data.Add(templateFIle.TemplateFinalise(this.User));
                    this.AjaxResult.Message = "Get user character summary success";
                    this.AjaxResult.Status = AjaxResultStatus.Success;
                }
                else
                {
                    this.AjaxResult.Message = "No user character summary data found";
                    this.AjaxResult.Status = AjaxResultStatus.Unassigned;
                }
            }
            else
            {
                this.AjaxResult.Message = "No user found";
                this.AjaxResult.Status = AjaxResultStatus.Unassigned;
            }
        }

        public void GetBlizzardCharacterProfile()
        {
            string characterName = ProviderBase.Framework.Utility.GetFormValue<string>("CharacterName", "");
            string realmName = ProviderBase.Framework.Utility.GetFormValue<string>("RealmName", "");
            string formBuilderGUID = ProviderBase.Framework.Utility.GetFormValue<string>("FormBuilder_GUID", "");
            string templateFinal = "";

            if (string.IsNullOrEmpty(formBuilderGUID) == false)
            {
                FormBuilderUtility formBuilderUtility = null;

                formBuilderUtility = new FormBuilderUtility(this.Website, formBuilderGUID);

                if (string.IsNullOrEmpty(characterName) == false)
                {
                    if (string.IsNullOrEmpty(realmName) == false)
                    {
                        ExternalCharacter characterBlizzardAPI = null;
                        ExternalCharacterProfile characterProfile = null;

                        characterBlizzardAPI = new ExternalCharacter(this.Website);

                        characterProfile = characterBlizzardAPI.GetCharacterProfile(characterName, realmName, CharacterAction.Items, CharacterAction.Stats, CharacterAction.Talents, CharacterAction.Guild, CharacterAction.Progression);

                        if (characterProfile != null)
                        {
                            if (characterProfile.Talents?.Count > 0)
                            {
                                characterProfile.Talents = characterProfile.Talents.Where(x => x.Selected == true).ToList<ExternalCharacterTalent>();

                                foreach (ExternalCharacterTalent talentList in characterProfile.Talents)
                                {
                                    talentList.Talents = talentList.Talents.OrderBy(x => x.Tier).ToList<ExternalCharacterTalentSpell>();
                                }
                            }

                            if (characterProfile?.Progression != null)
                            {
                                ExternalCharacterRaid externalCharacterRaid = null;

                                externalCharacterRaid = characterProfile.Progression.Raids.Last<ExternalCharacterRaid>();

                                characterProfile.Progression.Raids = new List<ExternalCharacterRaid>() { externalCharacterRaid };
                            }

                            templateFinal = formBuilderUtility.Render(characterProfile, this.User);

                            this.AjaxResult.Data.Add(templateFinal);
                            this.AjaxResult.Message = "Get forum group success";
                            this.AjaxResult.Status = AjaxResultStatus.Success;
                        }
                        else
                        {
                            this.AjaxResult.Message = "Character not found";
                            this.AjaxResult.Status = AjaxResultStatus.Failed;
                        }
                    }
                    else
                    {
                        this.AjaxResult.Message = "No realm name supplied";
                        this.AjaxResult.Status = AjaxResultStatus.Failed;
                    }
                }
                else
                {
                    this.AjaxResult.Message = "No character name supplied";
                    this.AjaxResult.Status = AjaxResultStatus.Failed;
                }
            }
        }
    }
}
