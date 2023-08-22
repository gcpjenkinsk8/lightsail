using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using DARReferenceData.ViewModels;
using FluentValidation;
using MySql.Data.MySqlClient;

namespace DARReferenceData.DatabaseHandlers.Validators
{
    public class AssetValidator : AbstractValidator<AssetViewModel>
    {
        public AssetValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;

            When(e => string.IsNullOrEmpty(e.DARAssetID), () =>
            {
                RuleFor(e => e.Name)
                    .NotEmpty()
                    .Length(2, 100).WithMessage("Length ({TotalLength}) of {PropertyName} Invalid")
                    .Must(BeAValidName).WithMessage("{PropertyValue} {PropertyName} contains invalid characters");

                RuleFor(e => e.DARTicker).NotEmpty();
            }).Otherwise(() =>
            {
                RuleFor(e => e.DARAssetID).Must(IdExists);
            });

            RuleFor(e => e.Description).Length(2, 1500).When(e => !string.IsNullOrEmpty(e.Description));
        }

        protected bool IdExists(string id)
        {
            string sql = $@"
                        SELECT *
                        FROM {DARApplicationInfo.SingleStoreCatalogInternal}.vAssetMaster a
                        where (LTRIM(RTRIM(DARTicker)) = @id
                        OR LTRIM(RTRIM(Name)) = @id
                        OR LTRIM(RTRIM(DARAssetID)) = @id
                        OR LTRIM(RTRIM(COALESCE(LegacyDARAssetId, 'X'))) = @id
                        OR LTRIM(RTRIM(COALESCE(LegacyId, '-1'))) = @id
                        )";

            var parameters = new DynamicParameters();
            parameters.Add("@id", id);

            AssetViewModel existing;

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                existing = connection.Query<AssetViewModel>(sql, parameters).FirstOrDefault();
            }

            return existing != null && existing.DARAssetID.Equals(id);
        }

        protected bool BeAValidName(string name)
        {
            return !name.Contains("'");
        }
    }
}