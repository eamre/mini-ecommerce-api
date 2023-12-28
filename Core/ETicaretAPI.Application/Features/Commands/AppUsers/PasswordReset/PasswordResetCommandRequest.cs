﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.AppUsers.PasswordReset
{
    public class PasswordResetCommandRequest:IRequest<PasswordResetCommandResponse>
    {
        public string Email { get; set; }
    }

}
