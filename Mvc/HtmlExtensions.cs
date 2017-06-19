﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Grammophone.Domos.Web.Mvc
{
	/// <summary>
	/// Extensions for <see cref="HtmlHelper"/>.
	/// </summary>
	public static class HtmlExtensions
	{
		/// <summary>
		/// Output an unescaped fragment when a boolean value is true.
		/// </summary>
		/// <param name="htmlHelper">The HTML helper.</param>
		/// <param name="value">The value to test.</param>
		/// <param name="fragment">The HTML fragment.</param>
		public static MvcHtmlString When(this HtmlHelper htmlHelper, bool value, string fragment)
		{
			if (htmlHelper == null) throw new ArgumentNullException("htmlHelper");
			if (fragment == null) throw new ArgumentNullException("fragment");

			if (value)
			{
				return new MvcHtmlString(fragment);
			}
			else
			{
				return MvcHtmlString.Empty;
			}
		}

		/// <summary>
		/// Determine whether a model field with any subproperties is valid.
		/// Model validation must have taken place before this call, 
		/// else the field is presumed always valid.
		/// </summary>
		/// <typeparam name="TModel">The type of the model.</typeparam>
		/// <typeparam name="TField">The type of the model's field.</typeparam>
		/// <param name="htmlHelper">The html helper.</param>
		/// <param name="fieldSelector">An expression specifying the field.</param>
		/// <returns>Returns whether the field is valid. If the field was not bound, it returns false.</returns>
		public static bool IsValid<TModel, TField>(
			this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TField>> fieldSelector)
		{
			if (htmlHelper == null) throw new ArgumentNullException("htmlHelper");
			if (fieldSelector == null) throw new ArgumentNullException("fieldSelector");

			string fieldPath = htmlHelper.NameFor(fieldSelector).ToString();

			return htmlHelper.ViewData.ModelState.IsValidField(fieldPath);
		}

		/// <summary>
		/// Output an html fragment when a model field with any subproperties is valid.
		/// Model validation must have taken place before this call, 
		/// else the field is presumed always valid.
		/// </summary>
		/// <typeparam name="TModel">The type of the model.</typeparam>
		/// <typeparam name="TField">The type of the model's field.</typeparam>
		/// <param name="htmlHelper">The html helper.</param>
		/// <param name="fieldSelector">An expression specifying the field.</param>
		/// <param name="fragment">The HTML fragment to output when the field is valid.</param>
		public static MvcHtmlString WhenValid<TModel, TField>(
			this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TField>> fieldSelector,
			string fragment)
		{
			bool isValid = IsValid(htmlHelper, fieldSelector);

			return When(htmlHelper, isValid, fragment);
		}

		/// <summary>
		/// Output an html fragment when a model field with any subproperties is not valid.
		/// Model validation must have taken place before this call, 
		/// else the field is presumed always valid.
		/// </summary>
		/// <typeparam name="TModel">The type of the model.</typeparam>
		/// <typeparam name="TField">The type of the model's field.</typeparam>
		/// <param name="htmlHelper">The html helper.</param>
		/// <param name="fieldSelector">An expression specifying the field.</param>
		/// <param name="fragment">The HTML fragment to output when the field is not valid.</param>
		public static MvcHtmlString WhenNotValid<TModel, TField>(
			this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TField>> fieldSelector,
			string fragment)
		{
			bool isValid = IsValid(htmlHelper, fieldSelector);

			return When(htmlHelper, !isValid, fragment);
		}

		/// <summary>
		/// When a field is not valid, outputs the bootstrap "has error" class name,
		/// else returns the empty string.
		/// </summary>
		/// <typeparam name="TModel">The type of the model.</typeparam>
		/// <typeparam name="TField">The type of the model's field.</typeparam>
		/// <param name="htmlHelper">The HTML helper.</param>
		/// <param name="modelPropertyExpression">Expression defining the model's field.</param>
		public static MvcHtmlString ValidationClassFor<TModel, TField>(
			this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TField>> modelPropertyExpression)
		{
			return htmlHelper.WhenNotValid(modelPropertyExpression, "has-error");
		}

		/// <summary>
		/// Show a partial view for a part of the view's model.
		/// </summary>
		/// <typeparam name="TModel">The type of the main model.</typeparam>
		/// <typeparam name="TField">The type of the part in the model.</typeparam>
		/// <param name="htmlHelper">The HTML helper.</param>
		/// <param name="partialViewName">The name of the partial view template.</param>
		/// <param name="modelPropertyExpression">An expression that defines the part inside the main model.</param>
		/// <returns>Returns the rendered partial view.</returns>
		/// <remarks>
		/// This method takes care of the model prefix which the partial view must include
		/// in its input field expressions.
		/// </remarks>
		public static MvcHtmlString PartialFor<TModel, TField>(
			this HtmlHelper<TModel> htmlHelper,
			string partialViewName,
			Expression<Func<TModel, TField>> modelPropertyExpression)
		{
			if (htmlHelper == null) throw new ArgumentNullException(nameof(htmlHelper));
			if (partialViewName == null) throw new ArgumentNullException(nameof(partialViewName));
			if (modelPropertyExpression == null) throw new ArgumentNullException(nameof(modelPropertyExpression));

			var fieldMetadata = ModelMetadata.FromLambdaExpression(modelPropertyExpression, htmlHelper.ViewData);

			return htmlHelper.Partial(
				partialViewName,
				fieldMetadata.Model,
				new ViewDataDictionary<TField>(htmlHelper.ViewData)
				{
					TemplateInfo = new TemplateInfo
					{
						HtmlFieldPrefix = fieldMetadata.PropertyName
					}
				});
		}
	}
}
