namespace CuentaClara.API.Models
{
    /// <summary>
    /// Clase base para todos los resultados de la API
    /// </summary>
    public class ApiResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }

        public ApiResult()
        {
            Success = true;
        }

        public ApiResult(string message)
        {
            Success = true;
            Message = message;
        }

        public ApiResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public ApiResult(bool success, List<string> errors)
        {
            Success = success;
            Errors = errors;
        }

        public static ApiResult Ok(string message = "Operación exitosa")
        {
            return new ApiResult(true, message);
        }

        public static ApiResult Fail(string error)
        {
            return new ApiResult(false, error);
        }

        public static ApiResult Fail(List<string> errors)
        {
            return new ApiResult(false, errors);
        }
    }

    /// <summary>
    /// Resultado de API genérico con datos tipados
    /// </summary>
    public class ApiResult<T> : ApiResult
    {
        public T? Data { get; set; }

        public ApiResult() : base()
        {
        }

        public ApiResult(T data) : base()
        {
            Data = data;
        }

        public ApiResult(T data, string message) : base(message)
        {
            Data = data;
        }

        public ApiResult(bool success, string message) : base(success, message)
        {
        }

        public ApiResult(bool success, List<string> errors) : base(success, errors)
        {
        }

        public static ApiResult<T> Ok(T data, string message = "Operación exitosa")
        {
            return new ApiResult<T>(data, message);
        }

        public new static ApiResult<T> Fail(string error)
        {
            return new ApiResult<T>
            {
                Success = false,
                Message = error
            };
        }

        public new static ApiResult<T> Fail(List<string> errors)
        {
            return new ApiResult<T>
            {
                Success = false,
                Errors = errors
            };
        }
    }
}
