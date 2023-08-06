using AutoMapper;
using Serilog;
using ApartmentManagement.Base;
using ApartmentManagement.Data.Domain;
using ApartmentManagement.Data.Uow;
using ApartmentManagement.Schema;
using ApartmentManagement.Business.Generic;
using ApartmentManagement.Base.Enums;

namespace ApartmentManagement.Business;

public class TransactionService : GenericService<Transaction, TransactionRequest, TransactionResponse>, ITransactionService
{

    private readonly IMapper mapper;
    private readonly IUnitOfWork unitOfWork;

    public TransactionService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
    {
        this.mapper = mapper;
        this.unitOfWork = unitOfWork;
    }

    public override ApiResponse Insert(TransactionRequest request)
    {
        var transaction = new Transaction();
        mapper.Map(request, transaction);
        transaction.Status = Enum_TransactionStatus.NotPaid;
        unitOfWork.TransactionRepository.Insert(transaction);
        unitOfWork.TransactionRepository.Save();
        return new ApiResponse();
    }
    public ApiResponse BulkInsert(TransactionMonthlyInputRequest request)
    {
        try
        {
            foreach (var item in request.Items)
            {
                var entity = new Transaction
                {
                    ApartmentId = item.ApartmentId,
                    Amount = item.Amount,
                    BillDate = request.BillDate,
                    BillType = item.BillType,
                    Status = Enum_TransactionStatus.NotPaid,

                };
                unitOfWork.TransactionRepository.Insert(entity);
            }
            unitOfWork.TransactionRepository.Save();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "TransactionService.BulkInsert");
            return new ApiResponse(ex.Message);
        }
        return new ApiResponse();
    }

    public ApiResponse BulkInsertAll(TransactionMonthlyInputAllRequest request)
    {
        try
        {
            var apartments = unitOfWork.ApartmentRepository.GetAll();
            foreach (var apartment in apartments)
            {
                var transaction = new Transaction();
                transaction.ApartmentId = apartment.Id;
                transaction.Amount = request.Amount;
                transaction.BillDate = request.BillDate;
                transaction.BillType = request.BillType;
                transaction.Status = Enum_TransactionStatus.NotPaid;
                unitOfWork.TransactionRepository.Insert(transaction);
            }
            unitOfWork.TransactionRepository.Save();

        }catch(Exception ex)
        {
            Log.Error(ex, "TransactionService.BulkInsertAll");
            return new ApiResponse(ex.Message);
        }

        return new ApiResponse();
    }

    public ApiResponse<List<TransactionResponse>> GetByApartmentId(short apartmentNumber)
    {
        try
        {
            var entityList = unitOfWork.TransactionRepository.WhereWithInclude(x => x.Apartment.ApartmentNumber == apartmentNumber, "Apartment").ToList();
            var mapped = mapper.Map<List<Transaction>, List<TransactionResponse>>(entityList);
            return new ApiResponse<List<TransactionResponse>>(mapped);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "TransactionService.GetByApartmentId");
            return new ApiResponse<List<TransactionResponse>>(ex.Message);
        }
    }

    public ApiResponse<List<TransactionResponse>> GetByBillDate(DateTime billDate, Enum_BillType? billType = null)
    {
        try
        {
            if (billType.HasValue)
            {
                var entityList = unitOfWork.TransactionRepository.WhereWithInclude(x => x.BillDate == billDate && x.BillType == billType, "Apartment").ToList();
                var mapped = mapper.Map<List<Transaction>, List<TransactionResponse>>(entityList);
                return new ApiResponse<List<TransactionResponse>>(mapped);
            } else {
                var entityList = unitOfWork.TransactionRepository.WhereWithInclude(x => x.BillDate == billDate, "Apartment").ToList();
                var mapped = mapper.Map<List<Transaction>, List<TransactionResponse>>(entityList);
                return new ApiResponse<List<TransactionResponse>>(mapped);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "TransactionService.GetByBillDate");
            return new ApiResponse<List<TransactionResponse>>(ex.Message);
        }
    }

    public ApiResponse<TransactionResponse> GetByReference(string referenceNumber)
    {
        try
        {
            var transaction = unitOfWork.TransactionRepository.WhereWithInclude(x=> x.ReferenceNumber == referenceNumber, "Apartment").FirstOrDefault();
            var mapped = mapper.Map<Transaction, TransactionResponse>(transaction);
            return new ApiResponse<TransactionResponse>(mapped);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "TransactionService.GetByReference");
            return new ApiResponse<TransactionResponse>(ex.Message);
        }
    }

    public ApiResponse<List<TransactionResponse>> GetByUserId(int userId)
    {
        try
        {
            var entityList = unitOfWork.TransactionRepository.WhereWithInclude(x => x.Apartment.UserId == userId, "Apartment").ToList();
            var mapped = mapper.Map<List<Transaction>, List<TransactionResponse>>(entityList);
            return new ApiResponse<List<TransactionResponse>>(mapped);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "TransactionService.GetByUserId");
            return new ApiResponse<List<TransactionResponse>>(ex.Message);
        }
    }

    public ApiResponse PaymentStart(int transactionId, string referenceNumber)
    {
        try
        {
            var transaction = unitOfWork.TransactionRepository.GetById(transactionId);
            if (transaction is null)
            {
                return new ApiResponse("Transaction not found");
            }
            transaction.ReferenceNumber = referenceNumber;
            transaction.Status = Enum_TransactionStatus.NotPaid;
            unitOfWork.TransactionRepository.Update(transaction);
            unitOfWork.TransactionRepository.Save();
            return new ApiResponse();

        }catch (Exception ex)
        {
            Log.Error(ex, "TransactionService.PaymentStart");
            return new ApiResponse(ex.Message);
        }
    }

    public ApiResponse PaymentComplete(int transactionId)
    {
        try 
        {
            var transaction = unitOfWork.TransactionRepository.GetById(transactionId);
            if (transaction is null)
            {
                return new ApiResponse("Transaction not found");
            }
            transaction.Status = Enum_TransactionStatus.Paid;
            unitOfWork.TransactionRepository.Update(transaction);
            unitOfWork.TransactionRepository.Save();
            return new ApiResponse();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "TransactionService.PaymentComplete");
            return new ApiResponse(ex.Message);
        }
    }
}