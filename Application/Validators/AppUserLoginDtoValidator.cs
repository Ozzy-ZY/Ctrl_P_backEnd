﻿using Application.DTOs.AuthModels;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators
{
    public class AppUserLoginDtoValidator : AbstractValidator<AppUserLoginDto>
    {
        public AppUserLoginDtoValidator()
        {
            RuleFor(x => x.UserName).NotEmpty()
                .WithMessage("Please Enter Valid Data")
                .Length(4, 64);
            RuleFor(x => x.Password).NotEmpty().WithMessage("Please Enter Valid Data");
        }
    }
}