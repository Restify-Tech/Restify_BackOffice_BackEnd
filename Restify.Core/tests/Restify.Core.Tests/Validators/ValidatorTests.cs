using FluentAssertions;
using FluentValidation.TestHelper;
using Restify.Core.Application.DTOs.DataProvider;
using Restify.Core.Application.DTOs.General;
using Restify.Core.Application.DTOs.Grid;
using Restify.Core.Application.Validators;

namespace Restify.Core.Tests.Validators;

#region GridConfiguration Validators

public class CreateGridConfigurationRequestValidatorTests
{
    private readonly CreateGridConfigurationRequestValidator _validator = new();

    [Fact]
    public async Task EmptyEntityName_ShouldFail()
    {
        var model = ValidRequest();
        model.EntityName = "";
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.EntityName);
    }

    [Fact]
    public async Task EmptyApiEndpoint_ShouldFail()
    {
        var model = ValidRequest();
        model.ApiEndpoint = "";
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.ApiEndpoint);
    }

    [Fact]
    public async Task PageSizeTooSmall_ShouldFail()
    {
        var model = ValidRequest();
        model.DefaultPageSize = 2;
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.DefaultPageSize);
    }

    [Fact]
    public async Task PageSizeTooLarge_ShouldFail()
    {
        var model = ValidRequest();
        model.DefaultPageSize = 200;
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.DefaultPageSize);
    }

    [Fact]
    public async Task EmptyDisplayName_ShouldFail()
    {
        var model = ValidRequest();
        model.DisplayName = "";
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.DisplayName);
    }

    [Fact]
    public async Task EmptyDisplayNamePlural_ShouldFail()
    {
        var model = ValidRequest();
        model.DisplayNamePlural = "";
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.DisplayNamePlural);
    }

    [Fact]
    public async Task ValidData_ShouldPass()
    {
        var model = ValidRequest();
        var result = await _validator.TestValidateAsync(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    private static CreateGridConfigurationRequest ValidRequest() => new()
    {
        EntityName = "Category",
        DisplayName = "Categoria",
        DisplayNamePlural = "Categorias",
        ApiEndpoint = "/api/categories",
        DefaultPageSize = 10
    };
}

public class UpdateGridConfigurationRequestValidatorTests
{
    private readonly UpdateGridConfigurationRequestValidator _validator = new();

    [Fact]
    public async Task EmptyId_ShouldFail()
    {
        var model = new UpdateGridConfigurationRequest
        {
            Id = Guid.Empty,
            EntityName = "Category",
            DisplayName = "Categoria",
            DisplayNamePlural = "Categorias",
            ApiEndpoint = "/api/categories",
            DefaultPageSize = 10
        };
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public async Task ValidData_ShouldPass()
    {
        var model = new UpdateGridConfigurationRequest
        {
            Id = Guid.NewGuid(),
            EntityName = "Category",
            DisplayName = "Categoria",
            DisplayNamePlural = "Categorias",
            ApiEndpoint = "/api/categories",
            DefaultPageSize = 10
        };
        var result = await _validator.TestValidateAsync(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

#endregion

#region GridColumn Validators

public class CreateGridColumnRequestValidatorTests
{
    private readonly CreateGridColumnRequestValidator _validator = new();

    [Fact]
    public async Task EmptyFieldName_ShouldFail()
    {
        var model = ValidRequest();
        model.FieldName = "";
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.FieldName);
    }

    [Fact]
    public async Task EmptyHeaderText_ShouldFail()
    {
        var model = ValidRequest();
        model.HeaderText = "";
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.HeaderText);
    }

    [Fact]
    public async Task EmptyGridConfigurationId_ShouldFail()
    {
        var model = ValidRequest();
        model.GridConfigurationId = Guid.Empty;
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.GridConfigurationId);
    }

    [Fact]
    public async Task NegativeGridOrder_ShouldFail()
    {
        var model = ValidRequest();
        model.GridOrder = -1;
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.GridOrder);
    }

    [Fact]
    public async Task ValidData_ShouldPass()
    {
        var model = ValidRequest();
        var result = await _validator.TestValidateAsync(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    private static CreateGridColumnRequest ValidRequest() => new()
    {
        GridConfigurationId = Guid.NewGuid(),
        FieldName = "name",
        HeaderText = "Nombre",
        GridOrder = 1,
        FormOrder = 1
    };
}

public class CreateGridColumnValidationRequestValidatorTests
{
    private readonly CreateGridColumnValidationRequestValidator _validator = new();

    [Fact]
    public async Task EmptyValidationType_ShouldFail()
    {
        var model = new CreateGridColumnValidationRequest
        {
            GridColumnId = Guid.NewGuid(),
            ValidationType = ""
        };
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.ValidationType);
    }

    [Fact]
    public async Task ValidData_ShouldPass()
    {
        var model = new CreateGridColumnValidationRequest
        {
            GridColumnId = Guid.NewGuid(),
            ValidationType = "Required",
            Order = 0
        };
        var result = await _validator.TestValidateAsync(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class CreateGridColumnLookupRequestValidatorTests
{
    private readonly CreateGridColumnLookupRequestValidator _validator = new();

    [Fact]
    public async Task EmptyTargetEntity_ShouldFail()
    {
        var model = ValidRequest();
        model.TargetEntity = "";
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.TargetEntity);
    }

    [Fact]
    public async Task EmptyApiEndpoint_ShouldFail()
    {
        var model = ValidRequest();
        model.ApiEndpoint = "";
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.ApiEndpoint);
    }

    [Fact]
    public async Task ValidData_ShouldPass()
    {
        var model = ValidRequest();
        var result = await _validator.TestValidateAsync(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    private static CreateGridColumnLookupRequest ValidRequest() => new()
    {
        GridColumnId = Guid.NewGuid(),
        TargetEntity = "Category",
        ApiEndpoint = "/api/categories",
        ValueField = "id",
        DisplayField = "name"
    };
}

#endregion

#region GeneralTable Validators

public class CreateGeneralTableRequestValidatorTests
{
    private readonly CreateGeneralTableRequestValidator _validator = new();

    [Fact]
    public async Task EmptyCode_ShouldFail()
    {
        var model = ValidRequest();
        model.Code = "";
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public async Task InvalidCodeFormat_ShouldFail()
    {
        var model = ValidRequest();
        model.Code = "invalid code!@#";
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public async Task EmptyName_ShouldFail()
    {
        var model = ValidRequest();
        model.Name = "";
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task CodeWithSpaces_ShouldFail()
    {
        var model = ValidRequest();
        model.Code = "HAS SPACES";
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public async Task ValidCodeWithHyphensAndUnderscores_ShouldPass()
    {
        var model = ValidRequest();
        model.Code = "VALID_CODE-123";
        var result = await _validator.TestValidateAsync(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public async Task ValidData_ShouldPass()
    {
        var model = ValidRequest();
        var result = await _validator.TestValidateAsync(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    private static CreateGeneralTableRequest ValidRequest() => new()
    {
        Code = "TIPOS_PAGO",
        Name = "Tipos de Pago"
    };
}

public class UpdateGeneralTableRequestValidatorTests
{
    private readonly UpdateGeneralTableRequestValidator _validator = new();

    [Fact]
    public async Task EmptyId_ShouldFail()
    {
        var model = new UpdateGeneralTableRequest
        {
            Id = Guid.Empty,
            Code = "VALID",
            Name = "Valid Name"
        };
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}

#endregion

#region GeneralValue Validators

public class CreateGeneralValueRequestValidatorTests
{
    private readonly CreateGeneralValueRequestValidator _validator = new();

    [Fact]
    public async Task EmptyCode_ShouldFail()
    {
        var model = ValidRequest();
        model.Code = "";
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public async Task EmptyContent_ShouldFail()
    {
        var model = ValidRequest();
        model.Content = "";
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Content);
    }

    [Fact]
    public async Task EmptyGeneralTableId_ShouldFail()
    {
        var model = ValidRequest();
        model.GeneralTableId = Guid.Empty;
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.GeneralTableId);
    }

    [Fact]
    public async Task InvalidCodeFormat_ShouldFail()
    {
        var model = ValidRequest();
        model.Code = "invalid code!";
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public async Task ValidData_ShouldPass()
    {
        var model = ValidRequest();
        var result = await _validator.TestValidateAsync(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    private static CreateGeneralValueRequest ValidRequest() => new()
    {
        GeneralTableId = Guid.NewGuid(),
        Code = "PENDING",
        Content = "Pendiente"
    };
}

public class UpdateGeneralValueRequestValidatorTests
{
    private readonly UpdateGeneralValueRequestValidator _validator = new();

    [Fact]
    public async Task EmptyId_ShouldFail()
    {
        var model = new UpdateGeneralValueRequest
        {
            Id = Guid.Empty,
            GeneralTableId = Guid.NewGuid(),
            Code = "VALID",
            Content = "Content"
        };
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}

public class GetValuesByTableCodeRequestValidatorTests
{
    private readonly GetValuesByTableCodeRequestValidator _validator = new();

    [Fact]
    public async Task EmptyTableCode_ShouldFail()
    {
        var model = new GetValuesByTableCodeRequest { TableCode = "" };
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.TableCode);
    }

    [Fact]
    public async Task ValidTableCode_ShouldPass()
    {
        var model = new GetValuesByTableCodeRequest { TableCode = "TIPOS_PAGO" };
        var result = await _validator.TestValidateAsync(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

#endregion

#region DataProvider Validators

public class DataProviderQueryRequestValidatorTests
{
    private readonly DataProviderQueryRequestValidator _validator = new();

    [Fact]
    public async Task PageLessThan1_ShouldFail()
    {
        var model = ValidRequest();
        model.Page = 0;
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Fact]
    public async Task PageSizeTooSmall_ShouldFail()
    {
        var model = ValidRequest();
        model.PageSize = 0;
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public async Task PageSizeTooLarge_ShouldFail()
    {
        var model = ValidRequest();
        model.PageSize = 101;
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public async Task InvalidSortDirection_ShouldFail()
    {
        var model = ValidRequest();
        model.SortDirection = "invalid";
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.SortDirection);
    }

    [Fact]
    public async Task ValidAscSortDirection_ShouldPass()
    {
        var model = ValidRequest();
        model.SortDirection = "asc";
        var result = await _validator.TestValidateAsync(model);
        result.ShouldNotHaveValidationErrorFor(x => x.SortDirection);
    }

    [Fact]
    public async Task ValidDescSortDirection_ShouldPass()
    {
        var model = ValidRequest();
        model.SortDirection = "desc";
        var result = await _validator.TestValidateAsync(model);
        result.ShouldNotHaveValidationErrorFor(x => x.SortDirection);
    }

    [Fact]
    public async Task EmptyEntityName_ShouldFail()
    {
        var model = ValidRequest();
        model.EntityName = "";
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.EntityName);
    }

    [Fact]
    public async Task ValidData_ShouldPass()
    {
        var model = ValidRequest();
        var result = await _validator.TestValidateAsync(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    private static DataProviderQueryRequest ValidRequest() => new()
    {
        EntityName = "Category",
        ViewName = "default",
        Page = 1,
        PageSize = 20,
        SortDirection = "asc"
    };
}

public class DataProviderMetadataRequestValidatorTests
{
    private readonly DataProviderMetadataRequestValidator _validator = new();

    [Fact]
    public async Task EmptyEntityName_ShouldFail()
    {
        var model = new DataProviderMetadataRequest { EntityName = "", ViewName = "default" };
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.EntityName);
    }

    [Fact]
    public async Task EmptyViewName_ShouldFail()
    {
        var model = new DataProviderMetadataRequest { EntityName = "Category", ViewName = "" };
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.ViewName);
    }

    [Fact]
    public async Task ValidData_ShouldPass()
    {
        var model = new DataProviderMetadataRequest { EntityName = "Category", ViewName = "default" };
        var result = await _validator.TestValidateAsync(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

#endregion
