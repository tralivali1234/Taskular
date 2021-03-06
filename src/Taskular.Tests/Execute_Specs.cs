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
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture]
    public class When_calling_execute
    {
        [Test]
        public async void Should_execute_the_method_immediately()
        {
            bool called = false;

            Task task = ComposerFactory.Compose(composer => composer.Execute(() => called = true, ExecuteOptions.RunSynchronously));

            await task;

            Assert.IsTrue(called);
        }

        [Test]
        public async void Should_execute_the_task()
        {
            bool called = false;

            Task task = ComposerFactory.Compose(composer => composer.Execute(() => called = true));

            await task;

            Assert.IsTrue(called);
        }
    }
}