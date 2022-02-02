namespace DevSpatium.IO.TextReader.Operations
{
    internal interface IVisitableOperation : IOperation
    {
        void AcceptVisitor(IOperationVisitor visitor);
    }
}