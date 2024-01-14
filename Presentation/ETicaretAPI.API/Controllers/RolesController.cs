using ETicaretAPI.Application.Consts;
using ETicaretAPI.Application.CustomAttributes;
using ETicaretAPI.Application.Enums;
using ETicaretAPI.Application.Features.Commands.Roles.CreateRole;
using ETicaretAPI.Application.Features.Commands.Roles.DeleteRole;
using ETicaretAPI.Application.Features.Commands.Roles.UpdateRole;
using ETicaretAPI.Application.Features.Queries.Roles.GetRoleById;
using ETicaretAPI.Application.Features.Queries.Roles.GetRoles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Admin")]

    public class RolesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RolesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Get Roles", Menu = AuthorizeDefinitionConstants.Roles)]
        public async Task<IActionResult> GetAllRoles([FromQuery] GetRolesQueryRequest request)
        {
            GetRolesQueryResponse response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpGet("{Id}")]
        [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Get Role by Id", Menu = AuthorizeDefinitionConstants.Roles)]
        public async Task<IActionResult> GetRole([FromRoute] GetRoleByIdQueryRequest request)
        {
            GetRoleByIdQueryResponse response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPost]
        [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "Create Role", Menu = AuthorizeDefinitionConstants.Roles)]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleCommandRequest request)
        {
            CreateRoleCommandResponse response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPut("{Id}")]
        [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "Update Role", Menu = AuthorizeDefinitionConstants.Roles)]
        public async Task<IActionResult> UpdateRole([FromBody, FromRoute] UpdateRoleCommandRequest request)
        {
            UpdateRoleCommandResponse response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpDelete("{Id}")]
        [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "Delete Role", Menu = AuthorizeDefinitionConstants.Roles)]
        public async Task<IActionResult> DeleteRole([FromRoute] DeleteRoleCommandRequest request)
        {
            DeleteRoleCommandResponse response = await _mediator.Send(request);
            return Ok(response);
        }
    }
}
