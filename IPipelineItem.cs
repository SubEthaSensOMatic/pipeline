using System;
using System.Threading.Tasks;

namespace Pipeline
{
    /// <summary>
    /// Item within the pipeline
    /// </summary>
    /// <typeparam name="T_PipelineContext">Pipeline context type</typeparam>
    public interface IPipelineItem<T_PipelineContext> 
    {
        /// <summary>
        /// Run pipeline item. Within the run method next() has to be called once.
        /// If not an exception will be thrown.
        /// </summary>
        /// <param name="ctx">Current pipeline context</param>
        /// <param name="next">Call next pipeline items</param>
        /// <returns></returns> 
        Task run(T_PipelineContext ctx, Func<Task> next);
    }
}
