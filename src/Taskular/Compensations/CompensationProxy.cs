// Copyright 2007-2014 Chris Patterson
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance
// with the License. You may obtain a copy of the License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed
// on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
namespace Taskular.Compensations
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public class CompensationProxy<T> :
        Compensation
    {
        readonly Compensation<T> _compensation;

        public CompensationProxy(Compensation<T> compensation)
        {
            _compensation = compensation;
        }

        public CancellationToken CancellationToken
        {
            get { return _compensation.CancellationToken; }
        }

        Exception Compensation.Exception
        {
            get { return _compensation.Exception; }
        }

        CompensationResult Compensation.Handled()
        {
            return new CompensationResultProxy<T>(_compensation.Handled());
        }

        CompensationResult Compensation.Task(Task task)
        {
            return new CompensationResultProxy<T>(_compensation.Task(task
                .ContinueWith(_ => _compensation.Payload, TaskContinuationOptions.ExecuteSynchronously)));
        }

        CompensationResult Compensation.Throw<TException>(TException exception)
        {
            return new CompensationResultProxy<T>(_compensation.Throw(exception));
        }

        CompensationResult Compensation.Throw()
        {
            return new CompensationResultProxy<T>(_compensation.Throw());
        }
    }
}