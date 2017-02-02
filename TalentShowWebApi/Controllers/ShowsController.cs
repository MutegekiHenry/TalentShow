﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TalentShow;
using TalentShow.Repos;
using TalentShow.Services;
using TalentShowDataStorage;
using TalentShowWebApi.DataTransferObjects;
using TalentShowWebApi.DataTransferObjects.Helpers;

namespace TalentShowWebApi.Controllers
{
    [Authorize]
    public class ShowsController : ApiController
    {
        private readonly ShowService ShowService;

        public ShowsController()
        {
            ShowService = new ShowService(new ShowRepo());
        }

        // GET api/Shows
        public IEnumerable<ShowDto> Get()
        {
            return ShowService.GetAll().ConvertToDto();
        }

        // GET api/Shows/5
        public ShowDto Get(int id)
        {
            return ShowService.Get(id).ConvertToDto();
        }

        // POST api/Shows
        public void Post([FromBody]ShowDto show)
        {
            ShowService.Add(show.ConvertFromDto());
        }

        // PUT api/Shows/5
        public void Put([FromBody]ShowDto show)
        {
            ShowService.Update(show.ConvertFromDto());
        }

        // DELETE api/Shows/5
        public void Delete(int id)
        {
            ShowService.Delete(id);
        }

        // DELETE api/Shows/5
        public void Delete([FromBody]ShowDto show)
        {
            ShowService.Delete(show.ConvertFromDto());
        }

        // DELETE api/Shows/
        public void Delete()
        {
            ShowService.DeleteAll();
        }
    }
}
