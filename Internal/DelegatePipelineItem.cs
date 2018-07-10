using System;
using System.Threading.Tasks;

namespace Pipeline.Internal
{
    internal class DelegatePipelineItem<T_PipelineContext> : IPipelineItem<T_PipelineContext>
    {
        private readonly Func<T_PipelineContext, Func<Task>, Task> _item;

        public DelegatePipelineItem(Func<T_PipelineContext, Func<Task>, Task> item)
        {
            _item = item ?? 
                throw new ArgumentNullException(nameof(item));
        }

        public Task run(T_PipelineContext ctx, Func<Task> next)
            => _item(ctx, next);
    }
}