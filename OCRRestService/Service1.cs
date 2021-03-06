﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;

namespace OCRRestService
{
    // Start the service and browse to http://<machine_name>:<port>/Service1/help to view the service's generated help page
    // NOTE: By default, a new instance of the service is created for each call; change the InstanceContextMode to Single if you want
    // a single instance of the service to process all calls.	
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    // NOTE: If the service is renamed, remember to update the global.asax.cs file
    public class Service1
    {

        [WebGet(UriTemplate = "?URL={url}")]
        public String GetCollection(string url)
        {
            ImageParser ip = new ImageParser();
            string s = ip.Parse(url);
            return s;
        }

        [WebInvoke(UriTemplate = "", Method = "POST")]
        public SampleItem Create(SampleItem instance)
        {
            // TODO: Add the new instance of SampleItem to the collection
            throw new NotImplementedException();
        }

        [WebGet(UriTemplate = "{id}")]
        public string Get(string id)
        {
            // TODO: Return the instance of SampleItem with the given id


            //Temporarily return a static JsonString for Roxanne's testing purposes, will eventually populate with data from schedule image
            List<SampleItem> SampleResultList = new List<SampleItem>();
            for(int i=0; i<10; i++)
            {
                // creating a sampleitem at a time and pushing them into a list

                SampleItem SampleItem = new SampleItem();
                SampleItem.CourseName = "Course No.: " + i.ToString();

                SampleItem.CourseLocation = "CourseLocation No.: " + i.ToString();
                SampleItem.CourseTime = new List<string>();

                for(int j=0; j<i ; j++)
                {
                    SampleItem.CourseTime.Add("DateTime: " + i.ToString() + " " + j.ToString());
                }

                SampleResultList.Add(SampleItem);
            }

            JsonOutput JsonOutput = new JsonOutput();

            string JsonResult = JsonOutput.ParseToJson(SampleResultList);

            return JsonResult;
        }

        [WebInvoke(UriTemplate = "{id}", Method = "PUT")]
        public SampleItem Update(string id, SampleItem instance)
        {
            // TODO: Update the given instance of SampleItem in the collection
            throw new NotImplementedException();
        }

        [WebInvoke(UriTemplate = "{id}", Method = "DELETE")]
        public void Delete(string id)
        {
            // TODO: Remove the instance of SampleItem with the given id from the collection
            throw new NotImplementedException();
        }

    }
}
