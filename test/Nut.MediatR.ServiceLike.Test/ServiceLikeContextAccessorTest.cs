using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Nut.MediatR.ServiceLike.Test
{
    public class ServiceLikeContextAccessorTest
    {
        [Fact]
        public void ctor_インスタンスを作成した時点ではコンテキストはnull()
        {
            var accessor = new ServiceLikeContextAccessor();
            accessor.Context.Should().BeNull();
        }

        [Fact]
        public async Task Context_設定したコンテキストが取得できる()
        {
            var accessor = new ServiceLikeContextAccessor();
            var context = new ServiceLikeContext("foo");

            accessor.Context = context;

            await Task.Delay(100);

            context.Should().BeSameAs(accessor.Context);
        }

        [Fact]
        public async Task Context_親のAsyncContextでnullに設定されたら子のコンテキストでもnullになる()
        {
            var accessor = new ServiceLikeContextAccessor();
            var context = new ServiceLikeContext("foo");
            accessor.Context = context;

            var checkAsyncFlowTcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
            var waitForNullTcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
            var afterNullCheckTcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

            ThreadPool.QueueUserWorkItem(async _ =>
            {
                context.Should().BeSameAs(accessor.Context);

                checkAsyncFlowTcs.SetResult(null);
                await waitForNullTcs.Task;

                try
                {
                    accessor.Context.Should().BeNull();
                    afterNullCheckTcs.SetResult(null);
                }
                catch (Exception ex)
                {
                    afterNullCheckTcs.SetException(ex);
                }
            });

            await checkAsyncFlowTcs.Task;
            accessor.Context = null;

            waitForNullTcs.SetResult(null);

            accessor.Context.Should().BeNull();

            await afterNullCheckTcs.Task;
        }

        [Fact]
        public async Task Context_親のAsyncContextで別のインスタンスが設定されたら子のコンテキストはnullになる()
        {
            var accessor = new ServiceLikeContextAccessor();
            var context = new ServiceLikeContext("foo");
            accessor.Context = context;

            var checkAsyncFlowTcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
            var waitForNullTcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
            var afterNullCheckTcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

            ThreadPool.QueueUserWorkItem(async _ =>
            {
                context.Should().BeSameAs(accessor.Context);
                checkAsyncFlowTcs.SetResult(null);

                await waitForNullTcs.Task;

                try
                {
                    accessor.Context.Should().BeNull();
                    afterNullCheckTcs.SetResult(null);
                }
                catch (Exception ex)
                {
                    afterNullCheckTcs.SetException(ex);
                }
            });

            await checkAsyncFlowTcs.Task;

            var context2 = new ServiceLikeContext("bar");
            accessor.Context = context2;

            waitForNullTcs.SetResult(null);

            context2.Should().BeSameAs(accessor.Context);

            await afterNullCheckTcs.Task;
        }

        [Fact]
        public async Task Context_親のAsyncContextにつながっていない場合は値は設定されない()
        {
            var accessor = new ServiceLikeContextAccessor();
            var context = new ServiceLikeContext("foo");
            accessor.Context = context;

            var checkAsyncFlowTcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

            ThreadPool.UnsafeQueueUserWorkItem(_ =>
            {
                try
                {
                    accessor.Context.Should().BeNull();
                    checkAsyncFlowTcs.SetResult(null);
                }
                catch (Exception ex)
                {
                    checkAsyncFlowTcs.SetException(ex);
                }
            }, null);

            await checkAsyncFlowTcs.Task;
        }
    }
}
