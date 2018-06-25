using System;
using ProviderBase.Framework.Handlers;
using ProviderBase.Framework.Forum.Entities;
using System.Collections.Generic;
using ProviderBase.Data.Providers;
using ProviderBase.Data.Entities;
using ProviderBase.Framework.Entities;

namespace ProviderBase.Framework.Forum.Handlers
{
    public class ForumHandler : BaseHandler
    {
        public void GetForumIndex()
        {
            string templatePrefix = ProviderBase.Framework.Utility.GetFormValue<string>("TemplatePrefix", "");
            string templateName = "ForumGroupRepeat.htm";
            string templateFinal = "";
            List<ForumGroup> forumGroupList = null;

            if (string.IsNullOrEmpty(templatePrefix))
            {
                templatePrefix = this.Website.WebsiteTemplatePrefix;
            }

            forumGroupList = DataProvider.SelectAll(new ForumGroup(), this.Website.WebsiteConnection.ConnectionString);

            if (forumGroupList?.Count > 0)
            {
                TemplateFile templateFileTemp = null;

                templateFileTemp = new TemplateFile(templatePrefix + templateName, this.Website.WebsiteTemplateFolder);

                foreach (ForumGroup forumGroupItem in forumGroupList)
                {
                    ForumGroupUserRoleLink forumGroupUserRoleLink = null;

                    forumGroupUserRoleLink = ProviderBase.Data.Providers.DataProvider.SelectSingleUsingDefault<ForumGroupUserRoleLink>(new ForumGroupUserRoleLink()
                    {
                        UserRoleTypeID = this.User.UserRoleTypeID,
                        ForumGroupID = forumGroupItem.ForumGroupID
                    }, this.Website.WebsiteConnection.ConnectionString, "UserRoleTypeID", "ForumGroupID");

                    if (forumGroupUserRoleLink?.UserRoleLevelID > 0)
                    {
                        List<ForumArea> forumAreaList = null;

                        forumAreaList = DataProvider.Select(new ForumArea()
                        {
                            ForumGroupID = forumGroupItem.ForumGroupID
                        }, this.Website.WebsiteConnection.ConnectionString);

                        if (forumAreaList?.Count > 0)
                        {
                            templateFileTemp.BindTemplateFileContentRepeat(forumGroupItem);

                            foreach (ForumArea forumAreaItem in forumAreaList)
                            {
                                int topicCount = 0;
                                int postCount = 0;
                                ForumThreadMessage forumThreadMessageNewest = null;
                                CharacterUser characterUserNewestForumThreadMessage = null;

                                topicCount = DataProvider.SelectCount<ForumArea, ForumThread>(forumAreaItem, this.Website.WebsiteConnection.ConnectionString);
                                postCount = DataProvider.SelectCount<ForumArea, ForumThreadMessage>(forumAreaItem, this.Website.WebsiteConnection.ConnectionString);

                                forumThreadMessageNewest = DataProvider.SelectNewest<ForumArea, ForumThreadMessage>(forumAreaItem, this.Website.WebsiteConnection.ConnectionString);

                                characterUserNewestForumThreadMessage = DataProvider.SelectSingleFull<CharacterUser>(new CharacterUser()
                                {
                                    CharacterUserID = forumThreadMessageNewest.CharacterUserID
                                }, this.Website.WebsiteConnection.ConnectionString);

                                templateFileTemp.BindTemplateFileContentRepeatItem(new List<object>() { forumAreaItem, forumThreadMessageNewest, characterUserNewestForumThreadMessage });

                                templateFileTemp.BindTemplateFileContentRepeatItemSingle("$FORUMTHREADCOUNT$", topicCount);
                                templateFileTemp.BindTemplateFileContentRepeatItemSingle("$FORUMTHREADMESSAGECOUNT$", postCount);
                            }

                            templateFileTemp.TemplateFinaliseRepeat();
                        }
                    }
                }

                templateFinal += templateFileTemp.TemplateFinalise(this.User);

                this.AjaxResult.Data.Add(templateFinal);
                this.AjaxResult.Message = "Get forum index success";
                this.AjaxResult.Status = AjaxResultStatus.Success;
            }

            else
            {
                this.AjaxResult.Message = "No forum index data found";
                this.AjaxResult.Status = AjaxResultStatus.Unassigned;
            }
        }

        public void GetForumArea()
        {
            int forumAreaID = ProviderBase.Framework.Utility.GetFormValue<int>("ID", 0);
            int pageCurrent = ProviderBase.Framework.Utility.GetFormValue<int>("PageCurrent", 1);
            int pageSize = ProviderBase.Framework.Utility.GetFormValue<int>("PageSize", 10);
            string templatePrefix = ProviderBase.Framework.Utility.GetFormValue<string>("TemplatePrefix", "");
            string templateName = "ForumAreaRepeat.htm";
            string templateFinal = "";

            if (string.IsNullOrEmpty(templatePrefix))
            {
                templatePrefix = this.Website.WebsiteTemplatePrefix;
            }

            if (forumAreaID > 0)
            {
                ForumArea forumAreaItem = null;
                DataProviderResultFilter paging = null;
                
                paging = new DataProviderResultFilter();
                paging.SetPaging(typeof(ForumArea), typeof(List<ForumThread>), pageCurrent, pageSize);

                forumAreaItem = ProviderBase.Data.Providers.DataProvider.SelectSingleFull(new ForumArea()
                {
                    ForumAreaID = forumAreaID
                }, this.Website.WebsiteConnection.ConnectionString, paging);

                if (forumAreaItem?.ForumAreaID > 0)
                {
                    ForumGroupUserRoleLink forumGroupUserRoleLink = null;

                    forumGroupUserRoleLink = ProviderBase.Data.Providers.DataProvider.SelectSingleUsingDefault<ForumGroupUserRoleLink>(new ForumGroupUserRoleLink()
                    {
                        UserRoleTypeID = this.User.UserRoleTypeID,
                        ForumGroupID = forumAreaItem.ForumGroupID
                    }, this.Website.WebsiteConnection.ConnectionString, "UserRoleTypeID", "ForumGroupID");

                    if (forumGroupUserRoleLink?.UserRoleLevelID > 0)
                    {
                        TemplateFile templateFileTemp = null;

                        templateFileTemp = new TemplateFile(templatePrefix + templateName, this.Website.WebsiteTemplateFolder);

                        templateFileTemp.BindTemplateFileContent(forumAreaItem);
                        templateFileTemp.BindTemplateFileContentRepeat(forumAreaItem);

                        if (forumAreaItem?.ThreadList.Count > 0)
                        {
                            PagingItem pagingItem = null;

                            pagingItem = ProviderBase.Data.Utility.XMLDeseralize<PagingItem>(this.Context.Server.MapPath(@"/Resource/XML/") + "Conquest/ConquestGamingPaging.xml");

                            templateFileTemp.BindTemplateFileContentSingle("$PAGING$", Utility.GeneratePaging(pagingItem, forumAreaItem.ThreadList[0].PageCurrent, forumAreaItem.ThreadList[0].PageTotal));
                            templateFileTemp.BindTemplateFileContentSingle("$PAGESIZE$", pageSize);

                            foreach (ForumThread forumThreadItem in forumAreaItem.ThreadList)
                            {
                                int postCount = 0;
                                int threadViews = 0;
                                CharacterUser characterUser = null;

                                postCount = DataProvider.SelectCount<ForumThread, ForumThreadMessage>(forumThreadItem, this.Website.WebsiteConnection.ConnectionString);
                                threadViews = DataProvider.SelectCount<ForumThread, ForumThreadView>(forumThreadItem, this.Website.WebsiteConnection.ConnectionString);

                                characterUser = DataProvider.SelectSingleFull<CharacterUser>(new CharacterUser()
                                {
                                    UserID = forumThreadItem.CharacterUserID
                                }, this.Website.WebsiteConnection.ConnectionString);

                                templateFileTemp.BindTemplateFileContentRepeatItem(new List<object>() { forumThreadItem, characterUser });

                                templateFileTemp.BindTemplateFileContentRepeatItemSingle("$FORUMTHREADMESSAGECOUNT$", (postCount - 1));
                                templateFileTemp.BindTemplateFileContentRepeatItemSingle("$FORUMTHREADVIEWCOUNT$", threadViews);
                            }
                        }
                        else
                        {
                            templateFileTemp.BindTemplateFileContentSingle("$PAGING$", "");
                        }

                        templateFinal += templateFileTemp.TemplateFinalise(this.User);

                        this.AjaxResult.Data.Add(templateFinal);
                        this.AjaxResult.Message = "Get forum area success";
                        this.AjaxResult.Status = AjaxResultStatus.Success;
                    }
                    else
                    {
                        this.AjaxResult.Redirect = Utility.GetRedirect(this.Context, RedirectReason.InvalidPermission);
                        this.AjaxResult.Status = AjaxResultStatus.Redirect;
                    }
                }
                else
                {
                    this.AjaxResult.Message = "No forum area found";
                    this.AjaxResult.Status = AjaxResultStatus.Failed;
                }
            }
            else
            {
                this.AjaxResult.Message = "No forum area ID supplied";
                this.AjaxResult.Status = AjaxResultStatus.Failed;
            }
        }

        public void GetForumGroup()
        {
            int forumGroupID = ProviderBase.Framework.Utility.GetFormValue<int>("ID", 0);
            string templatePrefix = ProviderBase.Framework.Utility.GetFormValue<string>("TemplatePrefix", "");
            string templateName = "ForumGroupRepeat.htm";
            string templateFinal = "";
            
            if (string.IsNullOrEmpty(templatePrefix))
            {
                templatePrefix = this.Website.WebsiteTemplatePrefix;
            }

            if (forumGroupID > 0)
            {
                ForumGroup forumGroupItem = null;

                forumGroupItem = DataProvider.SelectSingleFull(new ForumGroup()
                {
                    ForumGroupID = forumGroupID
                }, this.Website.WebsiteConnection.ConnectionString);

                if (forumGroupItem?.ForumGroupID > 0)
                {
                    ForumGroupUserRoleLink forumGroupUserRoleLink = null;

                    forumGroupUserRoleLink = ProviderBase.Data.Providers.DataProvider.SelectSingleUsingDefault<ForumGroupUserRoleLink>(new ForumGroupUserRoleLink()
                    {
                        UserRoleTypeID = this.User.UserRoleTypeID,
                        ForumGroupID = forumGroupItem.ForumGroupID
                    }, this.Website.WebsiteConnection.ConnectionString, "UserRoleTypeID", "ForumGroupID");

                    if (forumGroupUserRoleLink?.UserRoleLevelID > 0)
                    {
                        TemplateFile templateFileTemp = null;

                        templateFileTemp = new TemplateFile(templatePrefix + templateName, this.Website.WebsiteTemplateFolder);

                        templateFileTemp.BindTemplateFileContentRepeat(forumGroupItem);

                        foreach (ForumArea forumAreaItem in forumGroupItem.ThreadAreaList)
                        {
                            int topicCount = 0;
                            int postCount = 0;
                            ForumThreadMessage forumThreadMessageNewest = null;
                            CharacterUser characterUserNewestForumThreadMessage = null;

                            topicCount = DataProvider.SelectCount<ForumArea, ForumThread>(forumAreaItem, this.Website.WebsiteConnection.ConnectionString);
                            postCount = DataProvider.SelectCount<ForumArea, ForumThreadMessage>(forumAreaItem, this.Website.WebsiteConnection.ConnectionString);

                            forumThreadMessageNewest = DataProvider.SelectNewest<ForumArea, ForumThreadMessage>(forumAreaItem, this.Website.WebsiteConnection.ConnectionString);

                            characterUserNewestForumThreadMessage = DataProvider.SelectSingleFull<CharacterUser>(new CharacterUser()
                            {
                                CharacterUserID = forumThreadMessageNewest.CharacterUserID
                            }, this.Website.WebsiteConnection.ConnectionString);

                            templateFileTemp.BindTemplateFileContentRepeatItem(new List<object>() { forumAreaItem, forumThreadMessageNewest, characterUserNewestForumThreadMessage });

                            templateFileTemp.BindTemplateFileContentRepeatItemSingle("$FORUMTHREADCOUNT$", topicCount);
                            templateFileTemp.BindTemplateFileContentRepeatItemSingle("$FORUMTHREADMESSAGECOUNT$", postCount);
                        }

                        templateFinal += templateFileTemp.TemplateFinalise(this.User);

                        this.AjaxResult.Data.Add(templateFinal);
                        this.AjaxResult.Message = "Get forum group success";
                        this.AjaxResult.Status = AjaxResultStatus.Success;
                    }
                    else
                    {
                        this.AjaxResult.Redirect = Utility.GetRedirect(this.Context, RedirectReason.InvalidPermission);
                        this.AjaxResult.Status = AjaxResultStatus.Redirect;
                    }
                }
                else
                {
                    this.AjaxResult.Message = "No forum group found";
                    this.AjaxResult.Status = AjaxResultStatus.Failed;
                }
            }
            else
            {
                this.AjaxResult.Message = "No forum group ID supplied";
                this.AjaxResult.Status = AjaxResultStatus.Failed;
            }
        }

        public void GetForumThread()
        {
            int forumThreadID = ProviderBase.Framework.Utility.GetFormValue<int>("ID", 0);
            int pageCurrent = ProviderBase.Framework.Utility.GetFormValue<int>("PageCurrent", 1);
            int pageSize = ProviderBase.Framework.Utility.GetFormValue<int>("PageSize", 10);
            string templatePrefix = ProviderBase.Framework.Utility.GetFormValue<string>("TemplatePrefix", "");
            string templateName = "ForumThreadRepeat.htm";
            string templateFinal = "";
            
            if (string.IsNullOrEmpty(templatePrefix))
            {
                templatePrefix = this.Website.WebsiteTemplatePrefix;
            }

            if (forumThreadID > 0)
            {
                ForumThread forumThreadItem = null;
                DataProviderResultFilter paging = null;
                
                paging = new DataProviderResultFilter();
                paging.SetPaging(typeof(ForumArea), typeof(List<ForumThread>), pageCurrent, pageSize);

                forumThreadItem = DataProvider.SelectSingleFull(new ForumThread()
                {
                    ForumThreadID = forumThreadID
                }, this.Website.WebsiteConnection.ConnectionString, paging);

                if (forumThreadItem?.ForumThreadID > 0)
                {
                    ForumArea forumAreaItem = null;

                    forumAreaItem = ProviderBase.Data.Providers.DataProvider.SelectSingle<ForumArea>(new ForumArea()
                    {
                        ForumAreaID = forumThreadItem.ForumAreaID
                    }, this.Website.WebsiteConnection.ConnectionString);

                    if (forumAreaItem?.ForumAreaID > 0)
                    {
                        ForumGroupUserRoleLink forumGroupUserRoleLink = null;

                        forumGroupUserRoleLink = ProviderBase.Data.Providers.DataProvider.SelectSingleUsingDefault<ForumGroupUserRoleLink>(new ForumGroupUserRoleLink()
                        {
                            UserRoleTypeID = this.User.UserRoleTypeID,
                            ForumGroupID = forumAreaItem.ForumGroupID
                        }, this.Website.WebsiteConnection.ConnectionString, "UserRoleTypeID", "ForumGroupID");

                        if (forumGroupUserRoleLink?.UserRoleLevelID > 0)
                        {
                            TemplateFile templateFileTemp = null;
                            ForumThreadView forumThreadView = null;

                            templateFileTemp = new TemplateFile(templatePrefix + templateName, this.Website.WebsiteTemplateFolder);

                            forumThreadItem.PageSize = pageSize; // Override page size so BindTemplateFileContentRepeat is correct

                            templateFileTemp.BindTemplateFileContent(forumAreaItem, "FORUMAREA");
                            templateFileTemp.BindTemplateFileContent(forumThreadItem);
                            templateFileTemp.BindTemplateFileContentRepeat(forumThreadItem);

                            if (forumThreadItem.ThreadMessageList?.Count > 0)
                            {
                                PagingItem pagingItem = null;

                                pagingItem = ProviderBase.Data.Utility.XMLDeseralize<PagingItem>(this.Context.Server.MapPath(@"/Resource/XML/") + "Conquest/ConquestGamingPaging.xml");

                                templateFileTemp.BindTemplateFileContentRepeatSingle("$PAGING$", Utility.GeneratePaging(pagingItem, forumThreadItem.ThreadMessageList[0].PageCurrent, forumThreadItem.ThreadMessageList[0].PageTotal));
                                templateFileTemp.BindTemplateFileContentRepeatSingle("$PAGESIZE$", pageSize);

                                foreach (ForumThreadMessage forumThreadMessageItem in forumThreadItem.ThreadMessageList)
                                {
                                    CharacterUser characterUser = null;

                                    characterUser = DataProvider.SelectSingleFull<CharacterUser>(new CharacterUser()
                                    {
                                        UserID = forumThreadMessageItem.CharacterUserID
                                    }, this.Website.WebsiteConnection.ConnectionString);

                                    templateFileTemp.BindTemplateFileContentRepeatItem(new List<object>() { forumThreadMessageItem, characterUser });
                                }

                                forumThreadView = new ForumThreadView();
                                forumThreadView.CharacterUserID = this.User.UserID;
                                forumThreadView.ForumThreadID = forumThreadID;
                                forumThreadView.CreateDate = DateTime.Now;
                                forumThreadView.ForumThreadViewID = DataProvider.Insert<ForumThreadView>(forumThreadView, this.Website.WebsiteConnection.ConnectionString);
                            }
                            else
                            {
                                templateFileTemp.BindTemplateFileContentSingle("$PAGING$", "");
                            }

                            templateFinal += templateFileTemp.TemplateFinalise(this.User);

                            this.AjaxResult.Data.Add(templateFinal);
                            this.AjaxResult.Message = "Get forum thread success";
                            this.AjaxResult.Status = AjaxResultStatus.Success;
                        }
                        else
                        {
                            this.AjaxResult.Redirect = Utility.GetRedirect(this.Context, RedirectReason.InvalidPermission);
                            this.AjaxResult.Status = AjaxResultStatus.Redirect;
                        }
                    }
                }
                else
                {
                    this.AjaxResult.Message = "No forum thread found";
                    this.AjaxResult.Status = AjaxResultStatus.Failed;
                }
            }
            else
            {
                this.AjaxResult.Message = "No forum thread ID supplied";
                this.AjaxResult.Status = AjaxResultStatus.Failed;
            }
        }

        public void GetForumThreadNew()
        {
            int forumAreaID = ProviderBase.Framework.Utility.GetFormValue<int>("ID", 0);
            string templatePrefix = ProviderBase.Framework.Utility.GetFormValue<string>("TemplatePrefix", "");
            string templateName = "ForumThreadNew.htm";
            string templateFinal = "";

            if (string.IsNullOrEmpty(templatePrefix))
            {
                templatePrefix = this.Website.WebsiteTemplatePrefix;
            }

            if (forumAreaID > 0)
            {
                ForumArea forumArea = null;
                TemplateFile templateFileTemp = null;

                templateFileTemp = new TemplateFile(templatePrefix + templateName, this.Website.WebsiteTemplateFolder);

                forumArea = DataProvider.SelectSingle<ForumArea>(new ForumArea()
                {
                    ForumAreaID = forumAreaID
                }, this.Website.WebsiteConnection.ConnectionString);

                if (forumArea?.ForumAreaID > 0)
                {
                    ForumGroupUserRoleLink forumGroupUserRoleLink = null;

                    forumGroupUserRoleLink = ProviderBase.Data.Providers.DataProvider.SelectSingleUsingDefault<ForumGroupUserRoleLink>(new ForumGroupUserRoleLink()
                    {
                        UserRoleTypeID = this.User.UserRoleTypeID,
                        ForumGroupID = forumArea.ForumGroupID
                    }, this.Website.WebsiteConnection.ConnectionString, "UserRoleTypeID", "ForumGroupID");

                    if (forumGroupUserRoleLink?.UserRoleLevelID > 0)
                    {
                        templateFileTemp.BindTemplateFileContent(forumArea);

                        templateFinal += templateFileTemp.TemplateFinalise(this.User);

                        this.AjaxResult.Data.Add(templateFinal);
                        this.AjaxResult.Message = "Get forum thread success";
                        this.AjaxResult.Status = AjaxResultStatus.Success;
                    }
                    else
                    {
                        this.AjaxResult.Redirect = Utility.GetRedirect(this.Context, RedirectReason.InvalidPermission);
                        this.AjaxResult.Status = AjaxResultStatus.Redirect;
                    }
                }
            }
            else
            {
                this.AjaxResult.Message = "No forum area ID supplied";
                this.AjaxResult.Status = AjaxResultStatus.Failed;
            }
        }

        public void NewForumThread()
        {
            int forumAreaID = ProviderBase.Framework.Utility.GetFormValue<int>("ForumAreaID", 0);
            int forumThreadTypeID = ProviderBase.Framework.Utility.GetFormValue<int>("ForumThreadTypeID", 0);
            string forumThreadTitle = ProviderBase.Framework.Utility.GetFormValue<string>("ForumThreadTitle", "");
            string forumThreadMessageText = ProviderBase.Framework.Utility.GetFormValue<string>("ForumThreadMessage", "");
            string action = ProviderBase.Framework.Utility.GetFormValue<string>("Action", "");

            switch (action.ToLower())
            {
                case "submit":
                    if (forumAreaID > 0)
                    {
                        ForumArea forumAreaItem = null;
                        ForumGroupUserRoleLink forumGroupUserRoleLink = null;

                        forumAreaItem = ProviderBase.Data.Providers.DataProvider.SelectSingle<ForumArea>(new ForumArea()
                        {
                            ForumAreaID = forumAreaID
                        }, this.Website.WebsiteConnection.ConnectionString);

                        if (forumAreaItem?.ForumAreaID > 0)
                        {
                            forumGroupUserRoleLink = ProviderBase.Data.Providers.DataProvider.SelectSingleUsingDefault<ForumGroupUserRoleLink>(new ForumGroupUserRoleLink()
                            {
                                UserRoleTypeID = this.User.UserRoleTypeID,
                                ForumGroupID = forumAreaItem.ForumGroupID
                            }, this.Website.WebsiteConnection.ConnectionString, "UserRoleTypeID", "ForumGroupID");

                            if (forumGroupUserRoleLink?.UserRoleLevelID > 0)
                            {
                                if (string.IsNullOrEmpty(forumThreadTitle) == false)
                                {
                                    if (string.IsNullOrEmpty(forumThreadMessageText) == false)
                                    {
                                        CharacterUser characterUser = null;
                                        ForumThread forumThread = null;
                                        ForumThreadMessage forumThreadMessage = null;

                                        characterUser = ProviderBase.Data.Providers.DataProvider.SelectSingle<CharacterUser>(new CharacterUser()
                                        {
                                            UserID = this.User.UserID
                                        }, this.Website.WebsiteConnection.ConnectionString);

                                        forumThread = new ForumThread();
                                        forumThread.ForumAreaID = forumAreaID;
                                        forumThread.ForumThreadTypeID = (ForumThreadType)forumThreadTypeID;
                                        forumThread.CharacterUserID = characterUser.CharacterUserID;
                                        forumThread.Title = forumThreadTitle;
                                        forumThread.ForumThreadID = ProviderBase.Data.Providers.DataProvider.Insert<ForumThread>(forumThread, this.Website.WebsiteConnection.ConnectionString);

                                        forumThreadMessage = new ForumThreadMessage();
                                        forumThreadMessage.ForumThreadID = forumThread.ForumThreadID;
                                        forumThreadMessage.ForumThreadMessageTypeID = ForumThreadMessageType.Header;
                                        forumThreadMessage.CharacterUserID = characterUser.CharacterUserID;
                                        forumThreadMessage.MessageText = forumThreadMessageText;
                                        forumThreadMessage.ForumThreadMessageID = ProviderBase.Data.Providers.DataProvider.Insert<ForumThreadMessage>(forumThreadMessage, this.Website.WebsiteConnection.ConnectionString);

                                        this.AjaxResult.Data.Add(Convert.ToString(forumThread.ForumThreadID));

                                        this.AjaxResult.Status = AjaxResultStatus.Success;
                                        this.AjaxResult.Message = "New forum thread success";
                                    }
                                    else
                                    {
                                        this.AjaxResult.Status = AjaxResultStatus.Failed;
                                        this.AjaxResult.Message = "No forum thread message supplied";
                                    }
                                }
                                else
                                {
                                    this.AjaxResult.Status = AjaxResultStatus.Failed;
                                    this.AjaxResult.Message = "No forum thread title supplied";
                                }
                            }
                            else
                            {
                                this.AjaxResult.Redirect = Utility.GetRedirect(this.Context, RedirectReason.InvalidPermission);
                                this.AjaxResult.Status = AjaxResultStatus.Redirect;
                            }
                        }
                        else
                        {
                            this.AjaxResult.Status = AjaxResultStatus.Failed;
                            this.AjaxResult.Message = "Invalid forum area ID supplied";
                        }
                    }
                    else
                    {
                        this.AjaxResult.Status = AjaxResultStatus.Failed;
                        this.AjaxResult.Message = "No forum area ID supplied";
                    }
                    break;

                case "preview":
                    this.AjaxResult.Data.Add(NewForumThreadMessagePreview());
                    this.AjaxResult.Status = AjaxResultStatus.Success;
                    this.AjaxResult.Message = "New forum thread preview success";
                    break;
            }

                    
        }

        public void NewForumThreadMessage()
        {
            int forumThreadID = ProviderBase.Framework.Utility.GetFormValue<int>("ForumThreadID", 0);
            string forumThreadMessageText = ProviderBase.Framework.Utility.GetFormValue<string>("ForumThreadMessage", "");
            string action = ProviderBase.Framework.Utility.GetFormValue<string>("Action", "");

            switch (action.ToLower())
            {
                case "submit":
                    if (forumThreadID > 0)
                    {
                        ForumThread forumThreadItem = null;

                        forumThreadItem = ProviderBase.Data.Providers.DataProvider.SelectSingle<ForumThread>(new ForumThread()
                        {
                            ForumThreadID = forumThreadID
                        }, this.Website.WebsiteConnection.ConnectionString);

                        if (forumThreadItem?.ForumThreadID > 0)
                        {
                            ForumArea forumArea = null;

                            forumArea = ProviderBase.Data.Providers.DataProvider.SelectSingle<ForumArea>(new ForumArea()
                            {
                                ForumAreaID = forumThreadItem.ForumAreaID
                            }, this.Website.WebsiteConnection.ConnectionString);

                            if (forumArea?.ForumAreaID > 0)
                            {
                                ForumGroupUserRoleLink forumGroupUserRoleLink = null;

                                forumGroupUserRoleLink = ProviderBase.Data.Providers.DataProvider.SelectSingleUsingDefault<ForumGroupUserRoleLink>(new ForumGroupUserRoleLink()
                                {
                                    UserRoleTypeID = this.User.UserRoleTypeID,
                                    ForumGroupID = forumArea.ForumGroupID
                                }, this.Website.WebsiteConnection.ConnectionString, "UserRoleTypeID", "ForumGroupID");

                                if (forumGroupUserRoleLink?.UserRoleLevelID > 0)
                                {
                                    if (string.IsNullOrEmpty(forumThreadMessageText) == false)
                                    {
                                        CharacterUser characterUser = null;
                                        ForumThreadMessage forumThreadMessage = null;

                                        characterUser = ProviderBase.Data.Providers.DataProvider.SelectSingle<CharacterUser>(new CharacterUser()
                                        {
                                            UserID = this.User.UserID
                                        }, this.Website.WebsiteConnection.ConnectionString);

                                        forumThreadMessage = new ForumThreadMessage();
                                        forumThreadMessage.ForumThreadID = forumThreadID;
                                        forumThreadMessage.ForumThreadMessageTypeID = ForumThreadMessageType.Header;
                                        forumThreadMessage.CharacterUserID = characterUser.CharacterUserID;
                                        forumThreadMessage.MessageText = forumThreadMessageText;
                                        forumThreadMessage.ForumThreadMessageID = ProviderBase.Data.Providers.DataProvider.Insert<ForumThreadMessage>(forumThreadMessage, this.Website.WebsiteConnection.ConnectionString);

                                        this.AjaxResult.Status = AjaxResultStatus.Success;
                                        this.AjaxResult.Message = "New forum thread success";
                                    }
                                    else
                                    {
                                        this.AjaxResult.Status = AjaxResultStatus.Failed;
                                        this.AjaxResult.Message = "No forum thread message supplied";
                                    }
                                }
                                else
                                {
                                    this.AjaxResult.Redirect = Utility.GetRedirect(this.Context, RedirectReason.InvalidPermission);
                                    this.AjaxResult.Status = AjaxResultStatus.Redirect;
                                }
                            }
                            else
                            {
                                this.AjaxResult.Status = AjaxResultStatus.Failed;
                                this.AjaxResult.Message = "Invalid forum area";
                            }
                        }
                        else
                        {
                            this.AjaxResult.Status = AjaxResultStatus.Failed;
                            this.AjaxResult.Message = "Invalid forum thread ID supplied";
                        }
                    }
                    else
                    {
                        this.AjaxResult.Status = AjaxResultStatus.Failed;
                        this.AjaxResult.Message = "No forum thread ID supplied";
                    }
                    break;

                case "preview":
                    this.AjaxResult.Data.Add(NewForumThreadMessagePreview());
                    this.AjaxResult.Status = AjaxResultStatus.Success;
                    this.AjaxResult.Message = "New forum thread preview success";
                    break;
            }

            
        }

        private string NewForumThreadMessagePreview()
        {
            string forumThreadTitle = ProviderBase.Framework.Utility.GetFormValue<string>("ForumThreadTitle", "");
            string forumThreadMessageText = ProviderBase.Framework.Utility.GetFormValue<string>("ForumThreadMessage", "");
            string templatePrefix = ProviderBase.Framework.Utility.GetFormValue<string>("TemplatePrefix", "");
            string templateName = "ForumThreadRepeat.htm";
            string templateFinal = "";
            TemplateFile templateFileTemp = null;
            ForumThread forumThread = null;
            ForumThreadMessage forumThreadMessage = null;
            CharacterUser characterUser = null;

            if (string.IsNullOrEmpty(templatePrefix))
            {
                templatePrefix = this.Website.WebsiteTemplatePrefix;
            }

            templateFileTemp = new TemplateFile(templatePrefix + templateName, this.Website.WebsiteTemplateFolder);

            forumThread = new ForumThread();
            forumThread.CharacterUserID = this.User.UserID;
            forumThread.CreateDate = DateTime.Now;
            forumThread.Title = forumThreadTitle;

            forumThreadMessage = new ForumThreadMessage();
            forumThreadMessage.CharacterUserID = this.User.UserID;
            forumThreadMessage.CreateDate = DateTime.Now;
            forumThreadMessage.MessageText = forumThreadMessageText;

            characterUser = DataProvider.SelectSingleFull<CharacterUser>(new CharacterUser()
            {
                UserID = this.User.UserID
            }, this.Website.WebsiteConnection.ConnectionString);

            templateFileTemp.BindTemplateFileContent(forumThread);
            templateFileTemp.BindTemplateFileContentRepeat(forumThread);
            templateFileTemp.BindTemplateFileContentRepeatItem(new List<object>() { forumThreadMessage, characterUser });

            templateFinal += templateFileTemp.TemplateFinalise(this.User);

            return templateFinal;
        }
    }
}
