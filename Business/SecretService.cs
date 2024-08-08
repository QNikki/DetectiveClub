using DetectiveClub.Data;
using Environment = DetectiveClub.Data.Environment;

namespace DetectiveClub.Business;

internal class SecretService(
    IRepository<SecretType> secretTypes,
    IRepository<Secret> secrets,
    IRepository<Environment> environments) : ISecretService
{
    public StatusResult<ServiceStatus, int> AddSecret(Secret newSecret)
    {
        var isValid = ValidateSecret(newSecret);
        if (!isValid.Status)
        {
            return new StatusResult<ServiceStatus, int>(isValid.Result, -1);
        }

        var alreadyExist = secrets.GetAll()
            .Any(secret => secret.EnvironmentId == newSecret.EnvironmentId &&
                           secret.TypeId == newSecret.TypeId && secret.Сontent == newSecret.Сontent);

        if (alreadyExist)
        {
            return new StatusResult<ServiceStatus, int>(ServiceStatus.AlreadyExist, -1);
        }

        var secretId = secrets.Add(newSecret);
        return new StatusResult<ServiceStatus, int>(ServiceStatus.Success, secretId);
    }

    public ServiceStatus RemoveSecret(int secretId)
    {
        var secret = secrets.GetById(secretId);
        if (secret == null)
        {
            return ServiceStatus.NotFound;
        }

        secrets.Delete(secret);
        return ServiceStatus.Success;
    }

    public ServiceStatus EditSecret(Secret toEditSecret)
    {
        var secret = secrets.GetById(toEditSecret.Id);
        if (secret == null)
        {
            return ServiceStatus.NotFound;
        }

        var isValid = ValidateSecret(toEditSecret);
        if (!isValid.Status)
        {
            return isValid.Result;
        }

        secrets.Edit(secret);
        return ServiceStatus.Success;
    }

    public StatusResult<ServiceStatus, Secret> GetSecret(int secretId)
    {
        var secret = secrets.GetById(secretId);
        var status = secret is null ? ServiceStatus.NotFound : ServiceStatus.Success;
        return new(status, secret);
    }
    
    public StatusResult<ServiceStatus, IEnumerable<Secret>> GetSecretsByEnvironment(int environmentId)
    {
        if (!IsEnvironmentValid(environmentId))
        {
            return new StatusResult<ServiceStatus, IEnumerable<Secret>>(ServiceStatus.WrongEnvironment, default);
        }
        
        return new StatusResult<ServiceStatus, IEnumerable<Secret>>(ServiceStatus.Success,
            secrets.GetList(secret => secret.EnvironmentId == environmentId));
    }

    public StatusResult<ServiceStatus, IEnumerable<Secret>> GetSecretsByType(int typeId)
    {
        if (!IsTypeValid(typeId))
        {
            return new StatusResult<ServiceStatus, IEnumerable<Secret>>(ServiceStatus.WrongEnvironment, default);
        }

        return new StatusResult<ServiceStatus, IEnumerable<Secret>>(ServiceStatus.Success,
            secrets.GetList(secret => secret.TypeId == typeId));
    }

    public StatusResult<ServiceStatus, int> AddSecretType(SecretType newSecretType)
    {
        var alreadyExist = secretTypes.GetAll()
            .Any(secretType => secretType.Name == newSecretType.Name);

        if (alreadyExist)
        {
            return new StatusResult<ServiceStatus, int>(ServiceStatus.AlreadyExist, -1);
        }

        var secretId = secretTypes.Add(newSecretType);
        return new StatusResult<ServiceStatus, int>(ServiceStatus.Success, secretId);
    }

    public ServiceStatus RemoveSecretType(int secretTypeId)
    {
        var secretType = secretTypes.GetById(secretTypeId);
        if (secretType == null)
        {
            return ServiceStatus.NotFound;
        }

        var secretsByType = GetSecretsByType(secretTypeId).Result.ToList();
        foreach (var secret in secretsByType)
        {
            secrets.Delete(secret);
        }

        secretTypes.Delete(secretType);
        return ServiceStatus.Success;
    }

    public ServiceStatus EditSecretType(SecretType toEditSecretType)
    {
        var secretType = secretTypes.GetById(toEditSecretType.Id);
        if (secretType == null)
        {
            return ServiceStatus.NotFound;
        }

        secretTypes.Edit(toEditSecretType);
        return ServiceStatus.Success;
    }

    public StatusResult<ServiceStatus, SecretType> GetSecretType(int secretId)
    {
        var secretType = secretTypes.GetById(secretId);
        var status = secretType is null ? ServiceStatus.NotFound : ServiceStatus.Success;
        return new(status, secretType);
    }

    public IEnumerable<SecretType> GetSecretTypes() => secretTypes.GetAll();

    private StatusResult<bool, ServiceStatus> ValidateSecret(Secret target)
    {
        var result = ServiceStatus.Success;
        var status = true;
        if (!IsEnvironmentValid(target.EnvironmentId))
        {
            result = ServiceStatus.WrongEnvironment;
            status = false;
        }

        if (!IsTypeValid(target.TypeId))
        {
            result = ServiceStatus.WrongType;
            status = false;
        }

        return new StatusResult<bool, ServiceStatus>(status, result);
    }


    private bool IsEnvironmentValid(int environmentId) => environments.GetById(environmentId) is not null;

    private bool IsTypeValid(int typeId) => secretTypes.GetById(typeId) is not null;
}