using AutoMapper;
using DetectiveClub.Data;
using DetectiveClub.Business.Contracts;
using Environment = DetectiveClub.Data.Environment;

namespace DetectiveClub.Business;

internal class SecretService(
    IRepository<SecretType> secretTypes,
    IRepository<Secret> secrets,
    IRepository<Environment> environments) : ISecretService
{
    public StatusResult<ServiceStatus, int> AddSecret(SecretDto newSecret)
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

        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<SecretDto, Secret>())
            .CreateMapper();

        var secret = mapper.Map<Secret>(newSecret);
        var secretId = secrets.Add(secret);
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

    public ServiceStatus EditSecret(SecretDto toEditSecret)
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

    public StatusResult<ServiceStatus, SecretDto> GetSecret(int secretId)
    {
        var secret = secrets.GetById(secretId);
        var status = secret is null ? ServiceStatus.NotFound : ServiceStatus.Success;
        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Secret, SecretDto>())
            .CreateMapper();

        var secretDto = mapper.Map<SecretDto>(secret);
        return new(status, secretDto);
    }

    public StatusResult<ServiceStatus, IEnumerable<SecretDto>> GetSecretsByEnvironment(int environmentId)
    {
        if (!IsEnvironmentValid(environmentId))
        {
            return new StatusResult<ServiceStatus, IEnumerable<SecretDto>>(ServiceStatus.WrongEnvironment,
                new List<SecretDto>());
        }

        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<List<Secret>, List<SecretDto>>())
            .CreateMapper();

        var secretsByEnv = mapper
            .Map<List<SecretDto>>(secrets.GetList(secret => secret.EnvironmentId == environmentId));

        return new StatusResult<ServiceStatus, IEnumerable<SecretDto>>(ServiceStatus.Success, secretsByEnv);
    }

    public StatusResult<ServiceStatus, IEnumerable<SecretDto>> GetSecretsByType(int typeId)
    {
        if (!IsTypeValid(typeId))
        {
            return new StatusResult<ServiceStatus, IEnumerable<SecretDto>>(ServiceStatus.WrongType,
                new List<SecretDto>());
        }

        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<List<Secret>, List<SecretDto>>())
            .CreateMapper();

        var secretsByType = mapper.Map<List<SecretDto>>(secrets.GetList(secret => secret.TypeId == typeId));
        return new StatusResult<ServiceStatus, IEnumerable<SecretDto>>(ServiceStatus.Success, secretsByType);
    }

    public StatusResult<ServiceStatus, int> AddSecretType(SecretTypeDto newSecretType)
    {
        var alreadyExist = secretTypes.GetAll()
            .Any(secretType => secretType.Name == newSecretType.Name);

        if (alreadyExist)
        {
            return new StatusResult<ServiceStatus, int>(ServiceStatus.AlreadyExist, -1);
        }

        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<List<SecretTypeDto>, List<SecretType>>())
            .CreateMapper();

        var secretType = mapper.Map<SecretType>(newSecretType);
        var secretId = secretTypes.Add(secretType);
        return new StatusResult<ServiceStatus, int>(ServiceStatus.Success, secretId);
    }

    public ServiceStatus RemoveSecretType(int secretTypeId)
    {
        var secretType = secretTypes.GetById(secretTypeId);
        if (secretType == null)
        {
            return ServiceStatus.NotFound;
        }

        var secretsByTypeDto = GetSecretsByType(secretTypeId).Result.ToList();
        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<List<SecretDto>, List<Secret>>())
            .CreateMapper();

        var secretsByType = mapper.Map<List<Secret>>(secretsByTypeDto);
        foreach (var secret in secretsByType)
        {
            secrets.Delete(secret);
        }

        secretTypes.Delete(secretType);
        return ServiceStatus.Success;
    }

    public ServiceStatus EditSecretType(SecretTypeDto toEditSecretType)
    {
        var secretType = secretTypes.GetById(toEditSecretType.Id);
        if (secretType == null)
        {
            return ServiceStatus.NotFound;
        }

        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<SecretTypeDto, SecretType>())
            .CreateMapper();

        secretTypes.Edit(mapper.Map<SecretType>(toEditSecretType));
        return ServiceStatus.Success;
    }

    public StatusResult<ServiceStatus, SecretTypeDto> GetSecretType(int secretId)
    {
        var secretType = secretTypes.GetById(secretId);
        if (secretType is null)
        {
            return new(ServiceStatus.NotFound, new SecretTypeDto(-1, ""));
        }

        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<SecretType, SecretTypeDto>())
            .CreateMapper();

        return new(ServiceStatus.Success, mapper.Map<SecretTypeDto>(secretType));
    }

    public IEnumerable<SecretTypeDto> GetSecretTypes()
    {
        var secretTypesList = secretTypes.GetAll().ToList();
        var mapper = new MapperConfiguration(cfg => cfg.CreateMap<List<SecretType>, List<SecretTypeDto>>())
            .CreateMapper();

        return mapper.Map<List<SecretTypeDto>>(secretTypesList);
    }


    private StatusResult<bool, ServiceStatus> ValidateSecret(SecretDto target)
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