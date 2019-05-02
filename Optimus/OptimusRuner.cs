//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reactive.Linq;
//using Optimus.Contracts;
//using Optimus.Exceptions;
//using Optimus.Model;
//
//namespace Optimus
//{
//    public class OptimusRuner
//    {
//        private readonly IOptimizer _optimizer;
//        private readonly uint _tryCount;
//        private Queue<OptimusOperation> _queue;
//
//        public OptimusRuner(IOptimizer optimizer, uint tryCount)
//        {
//            _optimizer = optimizer ?? throw new ArgumentException(nameof(optimizer));
//            _tryCount = tryCount;
//        }
//
//        public IObservable<OptimusOperation> Run(IEnumerable<string> untrackedfiles)
//        {
//            var operations = untrackedfiles.Select(x => new OptimusOperation { SourcePath = x });
//            
//            _queue = new Queue<OptimusOperation>(operations);
//
//            return Observable.Create<OptimusOperation>(async observer =>
//            {
//                while (_queue.Count > 0)
//                {
//                    var next = _queue.Dequeue();
//
//                    try
//                    {
//                        next.TryCount++;
//                        var result = await _optimizer.Optimize(next);
//
////                        if (operation.Success)
////                            Log.Information($"[Optimized] {operation.SourcePath}");
////                        else
////                            Log.Error($"[Failed] {operation.SourcePath}");
//
//                        result.Success = true;
//                        observer.OnNext(result);
//                    }
//                    catch (ApiLimitException apiLimitException)
//                    {
//                        // an api limit exception means no more files can be processed
//                        observer.OnError(apiLimitException);
//                    }
//                    catch (Exception e)
//                    {
//                        if (next.ShouldRetry && next.TryCount < _tryCount)
//                        {
//                            _queue.Enqueue(next);
//                            continue;
//                        }
//
//                        next.Success = false;
//                        next.ShouldRetry = false;
//                        next.ErrorMessage = e.Message;
//                        
//                        observer.OnNext(next);
//                    }
//                }
//                
//                observer.OnCompleted();
//            });
//        }
//    }
//}