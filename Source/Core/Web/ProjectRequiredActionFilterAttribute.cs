﻿#region Copyright 2014 Exceptionless

// This program is free software: you can redistribute it and/or modify it 
// under the terms of the GNU Affero General Public License as published 
// by the Free Software Foundation, either version 3 of the License, or 
// (at your option) any later version.
// 
//     http://www.gnu.org/licenses/agpl-3.0.html

#endregion

using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Exceptionless.Core.Authorization;
using Exceptionless.Core.Utility;
using MongoDB.Bson;
using MongoDB.Driver.Builders;

namespace Exceptionless.Core.Web {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ProjectRequiredActionFilterAttribute : ActionFilterAttribute {
        [Inject]
        public IProjectRepository ProjectRepository { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context) {
            var user = context.HttpContext.User as ExceptionlessPrincipal;
            if (user != null && user.UserEntity != null) {
                long projects = ProjectRepository.Count(Query.In(Core.ProjectRepository.FieldNames.OrganizationId, user.GetAssociatedOrganizationIds().Select(id => new BsonObjectId(new ObjectId(id)))));
                if (projects == 0) {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary {
                        { "Controller", "Project" },
                        { "Action", "Add" }
                    });
                }
            }

            base.OnActionExecuting(context);
        }
    }
}