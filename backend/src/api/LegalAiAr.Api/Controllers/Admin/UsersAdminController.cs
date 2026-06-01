using LegalAiAr.Api.Models;
using LegalAiAr.Application.Admin.Users.Commands.CreateUser;
using LegalAiAr.Application.Admin.Users.Commands.DeleteUser;
using LegalAiAr.Application.Admin.Users.Commands.UpdateUser;
using LegalAiAr.Application.Admin.Users.DTOs;
using LegalAiAr.Application.Admin.Users.Queries.GetUsers;
using LegalAiAr.Application.Mediation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalAiAr.Api.Controllers.Admin;

/// <summary>
/// Admin API for user management.
/// </summary>
[ApiController]
[Route("api/admin/users")]
[Authorize]
public class UsersAdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersAdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// List all users.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<UserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetUsersQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Create a new user.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserDto>> Create(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Role))
            return BadRequest(new { error = "Email and Role are required." });

        var command = new CreateUserCommand(request.Email, request.DisplayName, request.Role);
        var result = await _mediator.Send(command, cancellationToken);
        return Created($"/api/admin/users/{result.Id}", result);
    }

    /// <summary>
    /// Update user (display name, role).
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> Update(
        Guid id,
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Role))
            return BadRequest(new { error = "Role is required." });

        var command = new UpdateUserCommand(id, request.DisplayName, request.Role);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Deactivate user (soft delete).
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand(id);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
