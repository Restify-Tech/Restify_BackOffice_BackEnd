using FluentAssertions;
using FluentValidation.TestHelper;
using Restify.Core.Application.DTOs.DataProvider;
using Restify.Core.Application.DTOs.General;
using Restify.Core.Application.DTOs.Grid;
using Restify.Core.Application.Validators;

namespace Restify.Core.Tests.Validators;

/// <summary>
/// Tests adicionales con Theory/InlineData y pruebas de limites para todos los validators de Core
/// </summary>
public class CoreValidatorTests
{
    #region GridConfiguration - Boundary Tests

    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(50)]
    [InlineData(100)]
    public async Task GridConfig_PageSizeWithinRange_ShouldPass(int pageSize)
    {
        var validator = new CreateGridConfigurationRequestValidator();
        var model = new CreateGridConfigurationRequest
        {
            EntityName = "Test",
            DisplayName = "Test",
            DisplayNamePlural = "Tests",
            ApiEndpoint = "/api/test",
            DefaultPageSize = pageSize
        };
        var result = await validator.TestValidateAsync(model);
        result.ShouldNotHaveValidationErrorFor(x => x.DefaultPageSize);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(4)]
    [InlineData(101)]
    [InlineData(200)]
    public async Task GridConfig_PageSizeOutOfRange_ShouldFail(int pageSize)
    {
        var validator = new CreateGridConfigurationRequestValidator();
        var model = new CreateGridConfigurationRequest
        {
            EntityName = "Test",
            DisplayName = "Test",
            DisplayNamePlural = "Tests",
            ApiEndpoint = "/api/test",
            DefaultPageSize = pageSize
        };
        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.DefaultPageSize);
    }

    [Fact]
    public async Task GridConfig_EntityNameExceeds100Chars_ShouldFail()
    {
        var validator = new CreateGridConfigurationRequestValidator();
        var model = new CreateGridConfigurationRequest
        {
            EntityName = new string('A', 101),
            DisplayName = "Test",
            DisplayNamePlural = "Tests",
            ApiEndpoint = "/api/test",
            DefaultPageSize = 10
        };
        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.EntityName);
    }

    [Fact]
    public async Task GridConfig_DisplayNameExceeds200Chars_ShouldFail()
    {
        var validator = new CreateGridConfigurationRequestValidator();
        var model = new CreateGridConfigurationRequest
        {
            EntityName = "Test",
            DisplayName = new string('A', 201),
            DisplayNamePlural = "Tests",
            ApiEndpoint = "/api/test",
            DefaultPageSize = 10
        };
        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.DisplayName);
    }

    [Fact]
    public async Task GridConfig_DescriptionExceeds500Chars_ShouldFail()
    {
        var validator = new CreateGridConfigurationRequestValidator();
        var model = new CreateGridConfigurationRequest
        {
            EntityName = "Test",
            DisplayName = "Test",
            DisplayNamePlural = "Tests",
            Description = new string('A', 501),
            ApiEndpoint = "/api/test",
            DefaultPageSize = 10
        };
        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public async Task GridConfig_NullDescription_ShouldPass()
    {
        var validator = new CreateGridConfigurationRequestValidator();
        var model = new CreateGridConfigurationRequest
        {
            EntityName = "Test",
            DisplayName = "Test",
            DisplayNamePlural = "Tests",
            Description = null,
            ApiEndpoint = "/api/test",
            DefaultPageSize = 10
        };
        var result = await validator.TestValidateAsync(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    #endregion

    #region GridColumn - Boundary Tests

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task GridColumn_NegativeGridOrder_ShouldFail(int order)
    {
        var validator = new CreateGridColumnRequestValidator();
        var model = new CreateGridColumnRequest
        {
            GridConfigurationId = Guid.NewGuid(),
            FieldName = "name",
            HeaderText = "Nombre",
            GridOrder = order,
            FormOrder = 0
        };
        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.GridOrder);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-50)]
    public async Task GridColumn_NegativeFormOrder_ShouldFail(int order)
    {
        var validator = new CreateGridColumnRequestValidator();
        var model = new CreateGridColumnRequest
        {
            GridConfigurationId = Guid.NewGuid(),
            FieldName = "name",
            HeaderText = "Nombre",
            GridOrder = 0,
            FormOrder = order
        };
        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.FormOrder);
    }

    [Fact]
    public async Task GridColumn_FieldNameExceeds100Chars_ShouldFail()
    {
        var validator = new CreateGridColumnRequestValidator();
        var model = new CreateGridColumnRequest
        {
            GridConfigurationId = Guid.NewGuid(),
            FieldName = new string('a', 101),
            HeaderText = "Nombre",
            GridOrder = 0,
            FormOrder = 0
        };
        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.FieldName);
    }

    [Fact]
    public async Task GridColumn_FormLabelExceeds200Chars_ShouldFail()
    {
        var validator = new CreateGridColumnRequestValidator();
        var model = new CreateGridColumnRequest
        {
            GridConfigurationId = Guid.NewGuid(),
            FieldName = "name",
            HeaderText = "Nombre",
            FormLabel = new string('a', 201),
            GridOrder = 0,
            FormOrder = 0
        };
        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.FormLabel);
    }

    #endregion

    #region GridColumnValidation - Boundary Tests

    [Fact]
    public async Task GridColumnValidation_EmptyGridColumnId_ShouldFail()
    {
        var validator = new CreateGridColumnValidationRequestValidator();
        var model = new CreateGridColumnValidationRequest
        {
            GridColumnId = Guid.Empty,
            ValidationType = "Required"
        };
        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.GridColumnId);
    }

    [Fact]
    public async Task GridColumnValidation_ValidationValueExceeds500Chars_ShouldFail()
    {
        var validator = new CreateGridColumnValidationRequestValidator();
        var model = new CreateGridColumnValidationRequest
        {
            GridColumnId = Guid.NewGuid(),
            ValidationType = "Regex",
            ValidationValue = new string('a', 501)
        };
        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.ValidationValue);
    }

    [Fact]
    public async Task GridColumnValidation_NegativeOrder_ShouldFail()
    {
        var validator = new CreateGridColumnValidationRequestValidator();
        var model = new CreateGridColumnValidationRequest
        {
            GridColumnId = Guid.NewGuid(),
            ValidationType = "Required",
            Order = -1
        };
        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Order);
    }

    [Fact]
    public async Task GridColumnValidation_ErrorMessageExceeds500Chars_ShouldFail()
    {
        var validator = new CreateGridColumnValidationRequestValidator();
        var model = new CreateGridColumnValidationRequest
        {
            GridColumnId = Guid.NewGuid(),
            ValidationType = "Required",
            ErrorMessage = new string('a', 501)
        };
        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.ErrorMessage);
    }

    #endregion

    #region GridColumnLookup - Boundary Tests

    [Fact]
    public async Task GridColumnLookup_EmptyGridColumnId_ShouldFail()
    {
        var validator = new CreateGridColumnLookupRequestValidator();
        var model = new CreateGridColumnLookupRequest
        {
            GridColumnId = Guid.Empty,
            TargetEntity = "Category",
            ApiEndpoint = "/api/categories",
            ValueField = "id",
            DisplayField = "name"
        };
        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.GridColumnId);
    }

    [Fact]
    public async Task GridColumnLookup_EmptyValueField_ShouldFail()
    {
        var validator = new CreateGridColumnLookupRequestValidator();
        var model = new CreateGridColumnLookupRequest
        {
            GridColumnId = Guid.NewGuid(),
            TargetEntity = "Category",
            ApiEndpoint = "/api/categories",
            ValueField = "",
            DisplayField = "name"
        };
        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.ValueField);
    }

    [Fact]
    public async Task GridColumnLookup_EmptyDisplayField_ShouldFail()
    {
        var validator = new CreateGridColumnLookupRequestValidator();
        var model = new CreateGridColumnLookupRequest
        {
            GridColumnId = Guid.NewGuid(),
            TargetEntity = "Category",
            ApiEndpoint = "/api/categories",
            ValueField = "id",
            DisplayField = ""
        };
        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.DisplayField);
    }

    [Fact]
    public async Task GridColumnLookup_TargetEntityExceeds100Chars_ShouldFail()
    {
        var validator = new CreateGridColumnLookupRequestValidator();
        var model = new CreateGridColumnLookupRequest
        {
            GridColumnId = Guid.NewGuid(),
            TargetEntity = new string('A', 101),
            ApiEndpoint = "/api/categories",
            ValueField = "id",
            DisplayField = "name"
        };
        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.TargetEntity);
    }

    #endregion

    #region GeneralTable - Code Format Tests

    [Theory]
    [InlineData("VALID_CODE")]
    [InlineData("code-123")]
    [InlineData("abc")]
    [InlineData("ABC123")]
    [InlineData("a-b_c")]
    public async Task GeneralTable_ValidCodeFormats_ShouldPass(string code)
    {
        var validator = new CreateGeneralTableRequestValidator();
        var model = new CreateGeneralTableRequest { Code = code, Name = "Test" };
        var result = await validator.TestValidateAsync(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Code);
    }

    [Theory]
    [InlineData("invalid code")]
    [InlineData("code!")]
    [InlineData("code@test")]
    [InlineData("code.test")]
    [InlineData("code+test")]
    public async Task GeneralTable_InvalidCodeFormats_ShouldFail(string code)
    {
        var validator = new CreateGeneralTableRequestValidator();
        var model = new CreateGeneralTableRequest { Code = code, Name = "Test" };
        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public async Task GeneralTable_CodeExceeds50Chars_ShouldFail()
    {
        var validator = new CreateGeneralTableRequestValidator();
        var model = new CreateGeneralTableRequest
        {
            Code = new string('A', 51),
            Name = "Test"
        };
        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public async Task GeneralTable_NameExceeds200Chars_ShouldFail()
    {
        var validator = new CreateGeneralTableRequestValidator();
        var model = new CreateGeneralTableRequest
        {
            Code = "VALID",
            Name = new string('A', 201)
        };
        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task GeneralTable_NegativeDisplayOrder_ShouldFail()
    {
        var validator = new CreateGeneralTableRequestValidator();
        var model = new CreateGeneralTableRequest
        {
            Code = "VALID",
            Name = "Test",
            DisplayOrder = -1
        };
        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.DisplayOrder);
    }

    #endregion

    #region GeneralValue - Boundary Tests

    [Fact]
    public async Task GeneralValue_CodeExceeds50Chars_ShouldFail()
    {
        var validator = new CreateGeneralValueRequestValidator();
        var model = new CreateGeneralValueRequest
        {
            GeneralTableId = Guid.NewGuid(),
            Code = new string('A', 51),
            Content = "Test"
        };
        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public async Task GeneralValue_ContentExceeds500Chars_ShouldFail()
    {
        var validator = new CreateGeneralValueRequestValidator();
        var model = new CreateGeneralValueRequest
        {
            GeneralTableId = Guid.NewGuid(),
            Code = "VALID",
            Content = new string('A', 501)
        };
        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Content);
    }

    [Fact]
    public async Task GeneralValue_ShortDescriptionExceeds200Chars_ShouldFail()
    {
        var validator = new CreateGeneralValueRequestValidator();
        var model = new CreateGeneralValueRequest
        {
            GeneralTableId = Guid.NewGuid(),
            Code = "VALID",
            Content = "Test",
            ShortDescription = new string('A', 201)
        };
        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.ShortDescription);
    }

    [Theory]
    [InlineData("Reference1")]
    [InlineData("Reference2")]
    [InlineData("Reference3")]
    [InlineData("Reference4")]
    [InlineData("Reference5")]
    public async Task GeneralValue_ReferenceExceeds500Chars_ShouldFail(string refField)
    {
        var validator = new CreateGeneralValueRequestValidator();
        var model = new CreateGeneralValueRequest
        {
            GeneralTableId = Guid.NewGuid(),
            Code = "VALID",
            Content = "Test"
        };

        var longValue = new string('A', 501);
        typeof(CreateGeneralValueRequest).GetProperty(refField)!.SetValue(model, longValue);

        var result = await validator.TestValidateAsync(model);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task GeneralValue_BackgroundColorExceeds20Chars_ShouldFail()
    {
        var validator = new CreateGeneralValueRequestValidator();
        var model = new CreateGeneralValueRequest
        {
            GeneralTableId = Guid.NewGuid(),
            Code = "VALID",
            Content = "Test",
            BackgroundColor = new string('A', 21)
        };
        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.BackgroundColor);
    }

    [Fact]
    public async Task GeneralValue_TextColorExceeds20Chars_ShouldFail()
    {
        var validator = new CreateGeneralValueRequestValidator();
        var model = new CreateGeneralValueRequest
        {
            GeneralTableId = Guid.NewGuid(),
            Code = "VALID",
            Content = "Test",
            TextColor = new string('A', 21)
        };
        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.TextColor);
    }

    [Fact]
    public async Task GeneralValue_NegativeDisplayOrder_ShouldFail()
    {
        var validator = new CreateGeneralValueRequestValidator();
        var model = new CreateGeneralValueRequest
        {
            GeneralTableId = Guid.NewGuid(),
            Code = "VALID",
            Content = "Test",
            DisplayOrder = -1
        };
        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.DisplayOrder);
    }

    [Theory]
    [InlineData("VALID_CODE")]
    [InlineData("code-123")]
    [InlineData("abc")]
    public async Task GeneralValue_ValidCodeFormats_ShouldPass(string code)
    {
        var validator = new CreateGeneralValueRequestValidator();
        var model = new CreateGeneralValueRequest
        {
            GeneralTableId = Guid.NewGuid(),
            Code = code,
            Content = "Test"
        };
        var result = await validator.TestValidateAsync(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Code);
    }

    #endregion

    #region DataProvider - Boundary Tests

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(100)]
    public async Task DataProvider_PageSizeWithinRange_ShouldPass(int pageSize)
    {
        var validator = new DataProviderQueryRequestValidator();
        var model = new DataProviderQueryRequest
        {
            EntityName = "Category",
            ViewName = "default",
            Page = 1,
            PageSize = pageSize,
            SortDirection = "asc"
        };
        var result = await validator.TestValidateAsync(model);
        result.ShouldNotHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public async Task DataProvider_GlobalSearchExceeds200Chars_ShouldFail()
    {
        var validator = new DataProviderQueryRequestValidator();
        var model = new DataProviderQueryRequest
        {
            EntityName = "Category",
            ViewName = "default",
            Page = 1,
            PageSize = 20,
            SortDirection = "asc",
            GlobalSearch = new string('A', 201)
        };
        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.GlobalSearch);
    }

    [Fact]
    public async Task DataProvider_EmptyViewName_ShouldFail()
    {
        var validator = new DataProviderQueryRequestValidator();
        var model = new DataProviderQueryRequest
        {
            EntityName = "Category",
            ViewName = "",
            Page = 1,
            PageSize = 20,
            SortDirection = "asc"
        };
        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.ViewName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("asc")]
    [InlineData("desc")]
    public async Task DataProvider_ValidSortDirections_ShouldPass(string sortDir)
    {
        var validator = new DataProviderQueryRequestValidator();
        var model = new DataProviderQueryRequest
        {
            EntityName = "Category",
            ViewName = "default",
            Page = 1,
            PageSize = 20,
            SortDirection = sortDir
        };
        var result = await validator.TestValidateAsync(model);
        result.ShouldNotHaveValidationErrorFor(x => x.SortDirection);
    }

    [Fact]
    public async Task DataProviderMetadata_EntityNameExceeds100Chars_ShouldFail()
    {
        var validator = new DataProviderMetadataRequestValidator();
        var model = new DataProviderMetadataRequest
        {
            EntityName = new string('A', 101),
            ViewName = "default"
        };
        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.EntityName);
    }

    [Fact]
    public async Task DataProviderMetadata_ViewNameExceeds100Chars_ShouldFail()
    {
        var validator = new DataProviderMetadataRequestValidator();
        var model = new DataProviderMetadataRequest
        {
            EntityName = "Category",
            ViewName = new string('A', 101)
        };
        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.ViewName);
    }

    #endregion
}
