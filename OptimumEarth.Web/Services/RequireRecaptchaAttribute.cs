using Microsoft.AspNetCore.Mvc.Filters;

namespace OptimumEarth.Web.Services;

/// <summary>
/// Put on a public page model whose form posts an enquiry. Every POST to the page is checked
/// with reCAPTCHA before its handler runs; a failed check adds a model error under
/// <see cref="ModelStateKey"/>, so the handler's existing <c>ModelState.IsValid</c> test
/// refuses the submission and the page re-renders with the message beside the widget.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public sealed class RequireRecaptchaAttribute : Attribute, IAsyncPageFilter
{
    public const string ModelStateKey = "Recaptcha";

    public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context) => Task.CompletedTask;

    public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
    {
        var request = context.HttpContext.Request;
        if (HttpMethods.IsPost(request.Method))
        {
            var token = request.HasFormContentType ? request.Form[RecaptchaService.ResponseField].ToString() : null;
            var recaptcha = context.HttpContext.RequestServices.GetRequiredService<RecaptchaService>();
            var result = await recaptcha.VerifyAsync(token, context.HttpContext.RequestAborted);
            var error = RecaptchaService.ErrorMessage(result);
            if (error is not null)
            {
                context.ModelState.AddModelError(ModelStateKey, error);
            }
        }

        await next();
    }
}
