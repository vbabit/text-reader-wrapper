namespace DevSpatium.IO.TextReader.Operations
{
    internal interface IOperationVisitor
    {
        void Visit(OneCharOperation operation);
        void Visit(CharBlockOperation operation);
        void Visit(RemainingLineOperation operation);
        void Visit(BoundaryStringOperation operation);
        void Visit(CompositeOperation operation);
        void Visit(DefaultOperationBlock operation);
        void Visit(ContinuousOperationBlock operation);
    }

    internal static class OperationVisitorExtensions
    {
        public static bool TryVisit(this IOperationVisitor visitor, IOperation operation)
        {
            if (!(operation is IVisitableOperation visitableOperation) || visitor == null)
            {
                return false;
            }

            visitableOperation.AcceptVisitor(visitor);
            return true;
        }
    }
}