using ProviderBase.Data.Entities;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Entities
{
    [DataProviderTable("FormBuilderFieldValidation_T")]
    public class FormBuilderFieldValidation
    {
        [DataProviderResultField("FormBuilderFieldValidationID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int FormBuilderFieldValidationID { get; set; }

        [DataProviderResultField("Title", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string Title { get; set; }

        [DataProviderResultField("ValidationRegex", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string ValidationRegex { get; set; }

        [DataProviderResultField("ValidationMessage", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string ValidationMessage { get; set; }

        public FormBuilderFieldValidation()
        {
            this.FormBuilderFieldValidationID = 0;
            this.Title = "";
            this.ValidationRegex = "";
            this.ValidationMessage = "";
        }
    }
}
