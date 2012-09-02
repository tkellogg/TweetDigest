﻿using System.Linq;
using System.Web.Mvc;
using Culminator.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Culminator.Controllers
{
    public class ErrorsController : Controller
    {
        private readonly MongoCollection<Error> db;

        public ErrorsController(MongoCollection<Error> db)
        {
            this.db = db;
        }

        //
        // GET: /Error/

        public ActionResult Index()
        {
            var errors = (from error in db.AsQueryable()
                          orderby error.Date descending
                          select error).Take(20).ToArray();
            return View(errors);
        }

    }
}
