using System.Threading.Tasks;
using static DevSpatium.IO.TextReader.ErrorMessages.Validation;

namespace DevSpatium.IO.TextReader.Operations
{
    internal sealed class OperationTreeValidator : IOperationVisitor
    {
        private bool _isRootPresent;
        private bool _isContinuousOperationBlockPresent;

        private OperationTreeValidator()
        {
        }

        public static void Validate(OperationTree operationTree)
        {
            operationTree.NotNull(nameof(operationTree));
            new OperationTreeValidator().Visit(operationTree);
        }

        public static Task ValidateAsync(OperationTree operationTree)
        {
            operationTree.NotNull(nameof(operationTree));
            return Task.Run(() => new OperationTreeValidator().Visit(operationTree));
        }

        void IOperationVisitor.Visit(OneCharOperation operation)
        {
            operation.NotNull(nameof(operation));
            ValidateOperation();
        }

        void IOperationVisitor.Visit(CharBlockOperation operation)
        {
            operation.NotNull(nameof(operation));
            ValidateOperation();
        }

        void IOperationVisitor.Visit(RemainingLineOperation operation)
        {
            operation.NotNull(nameof(operation));
            ValidateOperation();
        }

        void IOperationVisitor.Visit(BoundaryStringOperation operation)
        {
            operation.NotNull(nameof(operation));
            ValidateOperation();
        }

        void IOperationVisitor.Visit(CompositeOperation operation)
        {
            operation.NotNull(nameof(operation));
            ThrowIfRootMissing();
            ThrowIfFollowsAfterContinuousOperationBlock();
            foreach (var childOperation in operation.Operations)
            {
                ThrowIfContinuousOperationBlock(childOperation);
                this.TryVisit(childOperation);
            }

            ThrowIfCompositeEmpty(operation);
        }

        void IOperationVisitor.Visit(DefaultOperationBlock operation)
        {
            operation.NotNull(nameof(operation));
            ValidationOperationBlock(operation.Body);
            this.TryVisit(operation.Body);
        }

        void IOperationVisitor.Visit(ContinuousOperationBlock operation)
        {
            operation.NotNull(nameof(operation));
            ValidationOperationBlock(operation.Body);
            _isContinuousOperationBlockPresent = true;
        }

        private void Visit(OperationTree operationTree)
        {
            ThrowIfRootAlreadyPresent();
            _isRootPresent = true;
            foreach (var childOperation in operationTree.Operations)
            {
                this.TryVisit(childOperation);
            }

            ThrowIfCompositeEmpty(operationTree);
        }

        private void ValidateOperation()
        {
            ThrowIfRootMissing();
            ThrowIfFollowsAfterContinuousOperationBlock();
        }

        private void ValidationOperationBlock(IOperation child)
        {
            ThrowIfRootMissing();
            ThrowIfFollowsAfterContinuousOperationBlock();
            ThrowIfContinuousOperationBlock(child);
            this.TryVisit(child);
        }

        private void ThrowIfRootMissing()
        {
            if (!_isRootPresent)
            {
                throw Errors.Validation(MissingRoot);
            }
        }

        private void ThrowIfRootAlreadyPresent()
        {
            if (_isRootPresent)
            {
                throw Errors.Validation(DuplicateRoot);
            }
        }

        private void ThrowIfFollowsAfterContinuousOperationBlock()
        {
            if (_isContinuousOperationBlockPresent)
            {
                throw Errors.Validation(ContinuousOperationBlockFollowedByAnotherOperation);
            }
        }

        private static void ThrowIfContinuousOperationBlock(IOperation operation)
        {
            if (operation is ContinuousOperationBlock)
            {
                throw Errors.Validation(NestedContinuousOperationBlock);
            }
        }

        private static void ThrowIfCompositeEmpty(CompositeOperation operation)
        {
            if (operation.Operations.Count == 0)
            {
                throw Errors.Validation(operation.GetType().ToInvariantString(EmptyCompositeOperation));
            }
        }
    }
}