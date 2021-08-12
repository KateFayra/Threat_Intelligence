using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace QueryLib
{
    public interface IQuery
    {
        public Task<JObject> Query();
        public string GetName();
    }

}