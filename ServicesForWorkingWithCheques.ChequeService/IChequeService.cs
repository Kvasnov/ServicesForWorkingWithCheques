using System.Collections.Generic;
using System.ServiceModel;
using ServicesForWorkingWithCheques.Domain.Core;

namespace ServicesForWorkingWithCheques.ChequeService
{
    [ServiceContract]
    public interface IChequeService
    {
        [OperationContract]
        void SaveCheque( Cheque cheque );

        [OperationContract]
        IEnumerable< Cheque > GetChequeList( int chequeCount );
    }
}