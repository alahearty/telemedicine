using telemedicine_webapi.Application.Common.Exceptions;
using telemedicine_webapi.Application.TodoLists.Commands.CreateTodoList;
using telemedicine_webapi.Application.TodoLists.Commands.DeleteTodoList;
using telemedicine_webapi.Domain.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace telemedicine_webapi.Application.IntegrationTests.TodoLists.Commands;

using static Testing;

public class DeleteTodoListTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireValidTodoListId()
    {
        var command = new DeleteTodoListCommand(99);
        await FluentActions.Invoking(() => SendAsync(command)).Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task ShouldDeleteTodoList()
    {
        var listId = await SendAsync(new CreateTodoListCommand
        {
            Title = "New List"
        });

        await SendAsync(new DeleteTodoListCommand(listId));

        var list = await FindAsync<TodoList>(listId);

        list.Should().BeNull();
    }
}
