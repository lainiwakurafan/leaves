﻿using AutoMapper;
using AbcLeaves.Api.Models;
using AbcLeaves.Api.Services;
using System;

namespace AbcLeaves.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PostLeaveContract, ApplyLeaveContract>();
            CreateMap<ApplyLeaveContract, Leave>();
            CreateMap<PublishUserEventContract, AddCalendarEventContract>();
            CreateMap<Leave, GetLeavesItemContract>();
            CreateMap<Leave, PublishUserEventContract>().AfterMap((src, dst) => {
                dst.Title = LeaveEventDefaults.Title;
                dst.Description = LeaveEventDefaults.Description;
            });
            CreateMap<AddCalendarEventContract, CalendarEvent>().AfterMap((src, dst) =>
                dst.Summary = src.Title
            );
            CreateMap<DateTime, CalendarEventDateTime>()
                .ConstructUsing(dateTime => new CalendarEventDateTime(dateTime));
        }
    }
}
