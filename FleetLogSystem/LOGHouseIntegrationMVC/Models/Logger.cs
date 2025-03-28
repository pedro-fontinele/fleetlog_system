using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using LOGHouseSystem.Infra.Enums;

namespace LOGHouseSystem.Models
{
    public class Logger
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Descricao")]
        public string Description { get; set; }

        [DisplayName("Dados")]
        public string MetadataStr { get; set; }

        [DisplayName("Data operação")]
        public DateTime Date { get; set; }

        [DisplayName("Status")]
        public LogStatus Status { get; set; }

        public T GetMetaData<T>()
        {
            JObject obj = JObject.Parse(this.MetadataStr);
            return obj.ToObject<T>();
        }
    }
}
