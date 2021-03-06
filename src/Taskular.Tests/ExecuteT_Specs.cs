﻿// Copyright 2007-2014 Chris Patterson
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance
// with the License. You may obtain a copy of the License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed
// on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
namespace Taskular.Tests
{
    namespace ExecuteT_Specs
    {
        using System;
        using System.Threading;
        using System.Threading.Tasks;
        using NUnit.Framework;


        class Payload
        {
            public string Name { get; set; }
            public int Result { get; set; }
        }


        [TestFixture]
        public class When_executing_a_task_of_t
        {
            [Test]
            public async void Should_use_async_processing()
            {
                int threadId = Thread.CurrentThread.ManagedThreadId;

                int asyncThreadId = threadId;
                Task<Payload> task = ComposerFactory.Compose(new Payload {Name = "Chris"}, composer =>
                {
                    composer.Execute(x =>
                    {
                        x.Name = "Joe";
                        asyncThreadId = Thread.CurrentThread.ManagedThreadId;
                    });
                });

                Payload payload = await task;

                Assert.AreEqual("Joe", payload.Name);
                Assert.AreNotEqual(threadId, asyncThreadId);
            }

            [Test]
            public void Should_use_async_processing_and_capture_exceptions()
            {
                int threadId = Thread.CurrentThread.ManagedThreadId;
                int asyncThreadId = threadId;

                Task<Payload> task = ComposerFactory.Compose(new Payload {Name = "Chris"}, composer =>
                {
                    composer.ExecuteAsync((x,token) =>
                    {
                        x.Name = "Joe";
                        asyncThreadId = Thread.CurrentThread.ManagedThreadId;

                        throw new InvalidOperationException("This is expected");
                    });
                });

                Assert.Throws<InvalidOperationException>(async () => { Payload payload = await task; });

                Assert.AreNotEqual(threadId, asyncThreadId);
            }

            [Test]
            public void Should_call_an_async_method_nicely()
            {
                var task = ComposerFactory.Compose(new Payload(), composer =>
                {
                    composer.ExecuteAsync(async (payload, token) =>
                    {
                        var result = await SomeAsyncMethod(payload.Name);

                        payload.Result = result;
                    });
                });
            }

            async Task<int> SomeAsyncMethod(string value)
            {
                await ComposerFactory.Compose(composer => { });
                return 27;
            }

            [Test]
            public void Should_use_async_processing_and_capture_exceptions_synchronously()
            {
                int threadId = Thread.CurrentThread.ManagedThreadId;
                int asyncThreadId = threadId;

                Task<Payload> task = ComposerFactory.Compose(new Payload {Name = "Chris"}, composer =>
                {
                    composer.Execute(x =>
                    {
                        x.Name = "Joe";
                        asyncThreadId = Thread.CurrentThread.ManagedThreadId;

                        throw new InvalidOperationException("This is expected");
                    }, ExecuteOptions.RunSynchronously);
                });

                Assert.Throws<InvalidOperationException>(async () => { Payload payload = await task; });

                Assert.AreEqual(threadId, asyncThreadId, "Should have been on the same thread");
            }
        }
    }
}