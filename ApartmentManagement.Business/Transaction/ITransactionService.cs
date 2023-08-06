using ApartmentManagement.Base;
using ApartmentManagement.Base.Enums;
using ApartmentManagement.Business.Generic;
using ApartmentManagement.Data.Domain;
using ApartmentManagement.Schema;

namespace ApartmentManagement.Business;

public interface ITransactionService: IGenericService<Transaction,TransactionRequest,TransactionResponse>
{
    ApiResponse<TransactionResponse> GetByReference(string referenceNumber);

    /// <summary>
    /// Kullanıcı bazlı filtremele yapar.
    /// </summary>
    /// <param name="userId">kullanıcı kimlik numarası</param>
    /// <returns></returns>
    ApiResponse<List<TransactionResponse>> GetByUserId(int userId);

    /// <summary>
    /// Daire bazlı filtremele yapar.
    /// </summary>
    /// <param name="apartmentNumber">Daire No</param>
    /// <returns></returns>
    ApiResponse<List<TransactionResponse>> GetByApartmentId(short apartmentNumber);

    /// <summary>
    /// Fatura tarihine göre filtreleme yapar.
    /// </summary>
    /// <param name="billDate">Fatura Tarihi</param>
    /// <param name="billType">Fatura Tipi</param>
    /// <returns></returns>
    ApiResponse<List<TransactionResponse>> GetByBillDate(DateTime billDate, Enum_BillType? billType = null);

    /// <summary>
    /// Ay bazlı her bir daireye ait girişlerin daire bazlı toplu olarak girilmesi için kullanılır.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    ApiResponse BulkInsert(TransactionMonthlyInputRequest request);

    /// <summary>
    /// Ay bazlı her bir daireye ait girişlerin toplu olarak girilmesi için kullanılır.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    ApiResponse BulkInsertAll(TransactionMonthlyInputAllRequest request);


    ApiResponse PaymentStart(int transactionId, string referenceNumber);

    ApiResponse PaymentComplete(int transactionId);
}