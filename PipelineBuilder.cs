using Pipeline.Internal;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pipeline
{
    public class PipelineBuilder<T_PipelineContext>
    {
        private readonly List<Func<IPipelineItem<T_PipelineContext>>> _itemFactories
            = new List<Func<IPipelineItem<T_PipelineContext>>>();

        private readonly Func<Type, object> _itemResolver;

        private readonly object _builderLock = new object();

        /// <summary>
        /// Create new pipeline builder
        /// </summary>
        /// <param name="itemResolver">Resolver for items. Default resolver uses Activator.CreateInstance(...) </param>
        public PipelineBuilder(Func<Type, object> itemResolver = null)
        {
            _itemResolver = itemResolver ?? 
                new Func<Type, object> (type => Activator.CreateInstance(type));
        }

        /// <summary>
        /// Add item to pipeline
        /// </summary>
        /// <typeparam name="T_PipelineItem">Type of item</typeparam>
        /// <returns>The builder</returns>
        public PipelineBuilder<T_PipelineContext> Use<T_PipelineItem>() 
            where T_PipelineItem : IPipelineItem<T_PipelineContext>
        {
            lock (_builderLock)
            {
                _itemFactories.Add(() => 
                    (IPipelineItem<T_PipelineContext>)_itemResolver(typeof(T_PipelineItem)));

                return this;
            }
        }

        /// <summary>
        /// Item delegate item to pipeline
        /// </summary>
        /// <param name="item"></param>
        /// <returns>The builder</returns>
        public PipelineBuilder<T_PipelineContext> Use(Func<T_PipelineContext, Func<Task>, Task> item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            lock (_builderLock)
            {
                _itemFactories.Add(() => 
                    new DelegatePipelineItem<T_PipelineContext>(item));

                return this;
            }
        }

        /// <summary>
        /// Crearte callable to start running the pipeline
        /// </summary>
        /// <returns></returns>
        public Func<T_PipelineContext, Task> BuildPipeline()
        {
            lock (_builderLock)
            {
                // All next() statements called ?
                bool pipelineEndReached = false;

                // Last item
                Func<T_PipelineContext, Task> nextItem = _ =>
                {
                    pipelineEndReached = true;

                    return Task.CompletedTask;
                };

                for (var i = _itemFactories.Count - 1; i >= 0; i--)
                {
                    // Create item 
                    var item = _itemFactories[i]();

                    var next = nextItem;

                    nextItem = ctx =>
                    {
                        // next() statement called only once?
                        var nextCalled = false;

                        return item.run(ctx, async () =>
                        {
                            if (nextCalled)
                                throw new InvalidOperationException("next() statement executed multiple times!");

                            await next(ctx);

                            nextCalled = true;
                        });
                    };
                }

                return async ctx =>
                {
                    await nextItem(ctx);

                    if (!pipelineEndReached)
                        throw new InvalidOperationException("Pipeline end not reached. All next() statements must be executed!");
                };
            }
        }
    }
}
