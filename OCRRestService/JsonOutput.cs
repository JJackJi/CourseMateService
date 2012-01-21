using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;


namespace OCRRestService
{
    public class JsonOutput
    {
        //TO:DO have to be able to take in raw text parsed from an image and parse that into a list of sample items


        //takes in a list of SampleItem
        public string ParseToJson(List<SampleItem> ItemCollection)
        {
            JavaScriptSerializer JsonSerializer = new JavaScriptSerializer();
            string Json = JsonSerializer.Serialize(ItemCollection);
            return Json;
        }
    }
}