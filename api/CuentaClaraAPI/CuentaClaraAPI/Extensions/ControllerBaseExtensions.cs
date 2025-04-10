using CuentaClara.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CuentaClara.API.Extensions
{
    public static class ControllerBaseExtensions
    {
        public static ActionResult ApiResponse(this ControllerBase controller, ApiResult result)
        {
            if (result.Success)
            {
                return controller.Ok(result);
            }

            // Errores comunes
            if (result.Message?.Contains("no encontrado", StringComparison.OrdinalIgnoreCase) == true ||
                result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
            {
                return controller.NotFound(result);
            }

            if (result.Message?.Contains("no autorizado", StringComparison.OrdinalIgnoreCase) == true ||
                result.Message?.Contains("unauthorized", StringComparison.OrdinalIgnoreCase) == true)
            {
                return controller.Unauthorized(result);
            }

            // Por defecto, devolvemos BadRequest
            return controller.BadRequest(result);
        }

        public static ActionResult ApiResponse<T>(this ControllerBase controller, ApiResult<T> result)
        {
            if (result.Success)
            {
                if (result.Data == null)
                {
                    return controller.NoContent();
                }
                return controller.Ok(result);
            }

            // Errores comunes
            if (result.Message?.Contains("no encontrado", StringComparison.OrdinalIgnoreCase) == true ||
                result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
            {
                return controller.NotFound(result);
            }

            if (result.Message?.Contains("no autorizado", StringComparison.OrdinalIgnoreCase) == true ||
                result.Message?.Contains("unauthorized", StringComparison.OrdinalIgnoreCase) == true)
            {
                return controller.Unauthorized(result);
            }

            // Por defecto, devolvemos BadRequest
            return controller.BadRequest(result);
        }

        // Métodos de ayuda para casos comunes
        public static ActionResult OkResult(this ControllerBase controller, string message = "Operación exitosa")
        {
            return controller.Ok(ApiResult.Ok(message));
        }

        public static ActionResult OkResult<T>(this ControllerBase controller, T data, string message = "Operación exitosa")
        {
            return controller.Ok(ApiResult<T>.Ok(data, message));
        }

        public static ActionResult CreatedResult<T>(this ControllerBase controller, string actionName, object routeValues, T data)
        {
            var result = ApiResult<T>.Ok(data, "Recurso creado exitosamente");
            return controller.CreatedAtAction(actionName, routeValues, result);
        }

        public static ActionResult BadRequestResult(this ControllerBase controller, string error)
        {
            return controller.BadRequest(ApiResult.Fail(error));
        }

        public static ActionResult BadRequestResult(this ControllerBase controller, List<string> errors)
        {
            return controller.BadRequest(ApiResult.Fail(errors));
        }

        public static ActionResult NotFoundResult(this ControllerBase controller, string message = "Recurso no encontrado")
        {
            return controller.NotFound(ApiResult.Fail(message));
        }

        public static ActionResult UnauthorizedResult(this ControllerBase controller, string message = "No autorizado")
        {
            return controller.Unauthorized(ApiResult.Fail(message));
        }
    }
}
