using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Xunit;

namespace Nut.MediatR.ServiceLike.Test;

public class ServiceLikeMediatorTest
{
    [Fact]
    public async Task PublishCore_全てのハンドラが非同期で実行され完了を待機されない()
    {
        var testMediator = new TestServiceLikeMediator();
        var taskList = new List<Task>();
        var messageList = new List<string>();
        var ping = new TestPing();

        Func<INotification, CancellationToken, Task> CreateFunc(string id, int wait)
        {
            var task = new Task(() =>
            {
                Thread.Sleep(wait);
                messageList.Add(id);
            });
            taskList.Add(task);

            return new Func<INotification, CancellationToken, Task>((ev, ct) =>
            {
                ev.Should().BeSameAs(ping);
                task.Start();
                return task;
            });
        }

        await testMediator.Run(new[] { CreateFunc("1", 350), CreateFunc("2", 500), CreateFunc("3", 200) }, ping);
        await Task.WhenAll(taskList);
        messageList.Should().HaveCount(3).And.Contain("1", "2", "3");

    }

    private class TestPing : INotification
    {
    }
}

internal class TestServiceLikeMediator : ServiceLikeMediator
{
    public TestServiceLikeMediator() : base(new ServiceFactory(_ => null))
    {
    }

    public Task Run(IEnumerable<Func<INotification, CancellationToken, Task>> allHandlers, INotification notification)
    {
        return PublishCore(allHandlers, notification, new CancellationToken());
    }
}
