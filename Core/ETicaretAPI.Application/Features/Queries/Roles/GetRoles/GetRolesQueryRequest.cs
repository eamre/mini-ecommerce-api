﻿using ETicaretAPI.Application.RequestParameters;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Queries.Roles.GetRoles
{
    public class GetRolesQueryRequest:IRequest<GetRolesQueryResponse>
    {
        public Pagination Pagination { get; set; } = new Pagination();

    }
}
