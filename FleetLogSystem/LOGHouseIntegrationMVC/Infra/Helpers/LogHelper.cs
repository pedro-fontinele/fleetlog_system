using LOGHouseSystem.Infra.Helpers;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories;
using Newtonsoft.Json.Linq;

namespace LOGHouseSystem
{
    public static class Log
    {
        public static void Error(string description, object metadata = null)
        {
            LoggerRepository _logRepository = new LoggerRepository();
            var defaultMdt = new DefaultMetaData { Description = description };
            var obj = new Logger()
            {
                Description = description,
                Status = Infra.Enums.LogStatus.Erro,
                Date = DateTime.Now,
                MetadataStr = metadata is string ? metadata.ToString() : JObject.FromObject(metadata ?? defaultMdt).ToString()
            };

            _logRepository.Add(obj);
        }

        public static void Success(string description, object metadata = null)
        {
            LoggerRepository _logRepository = new LoggerRepository();
            var defaultMdt = new DefaultMetaData { Description = description };
            var obj = new Logger()
            {
                Description = description,
                Status = Infra.Enums.LogStatus.Sucesso,
                Date = DateTime.Now,
                MetadataStr = JObject.FromObject(metadata ?? defaultMdt).ToString()
            };

            _logRepository.Add(obj);
        }

        public static void Info(string description, object metadata = null)
        {
            LoggerRepository _logRepository = new LoggerRepository();
            var defaultMdt = new DefaultMetaData { Description = description };
            var obj = new Logger()
            {
                Description = description,
                Status = Infra.Enums.LogStatus.Info,
                Date = DateTime.Now,
                MetadataStr = JObject.FromObject(metadata ?? defaultMdt).ToString()
            };

            _logRepository.Add(obj);
        }
    }

    public class LogService { 
        public void CleanLogger()
        {
            var EndDate = DateTimeHelper.GetCurrentDateTime().AddDays(-5);
            //var monthAgo = DateTime.Now.AddMonths(-1);

            LoggerRepository _logRepository = new LoggerRepository();
            int counterToDeleting = 0;
            do
            {
                var registerForDelete = _logRepository.GetByDate(EndDate);
                counterToDeleting = registerForDelete.Count;

                if (registerForDelete.Count > 0)
                    _logRepository.DeleteRange(registerForDelete.ToList());
            } while (counterToDeleting > 0);

            
            //var deleteInfoWeek = _logRepository.GetByDateAndStatus(weekAgo, Infra.Enums.LogStatus.Info);
            //_logRepository.DeleteRange(deleteInfoWeek);

            //var deleteSuccessWeek = _logRepository.GetByDateAndStatus(weekAgo, Infra.Enums.LogStatus.Sucesso);
            //_logRepository.DeleteRange(deleteSuccessWeek);

        }
    }

    public class DefaultMetaData
    {
        public string Description { get; set; }
    }
}
