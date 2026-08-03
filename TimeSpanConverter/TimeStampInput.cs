using DevToys.Api;
using static DevToys.Api.GUI;

namespace TimeSpanConverter
{
    internal class TimeSpanInput
    {
        private static readonly HashSet<TimeSpanInput> _allTimeSpanInputs = [];

        private readonly TimeSpanInputMember[] _members;
        private readonly TimeSpanInputMember? _singleMember;

        private TimeSpan? _value;

        public TimeSpan Value
        {
            get =>
                _value ??
                (_singleMember is { } singleInput ?
                DoubleToTimeSpan(singleInput.Input.Value, singleInput.Format)
                    : _members.Select(i => DoubleToTimeSpan(i.Input.Value, i.Format))
                        .Aggregate((tA, tB) => tA + tB));
            set
            {
                if (_singleMember is { } singleInput)
                {
                    singleInput.Input.Value(TimeSpanToDouble(value, singleInput.Format));
                }
                else
                {
                    SetMemberValuesFromTimeStamp(value, _members);
                }
                _value = value;
            }
        }

        private bool ReadOnly 
        {
            get => _members.Any(m => m.Input.IsReadOnly);
            set
            {
                foreach (var member in _members)
                {
                    if (value)
                    {
                        member.Input.ReadOnly();
                    }
                    else
                    {
                        member.Input.Editable();
                    }
                }
            }
        }

        public IUIElement AsUIElement() => new Lazy<IUIElement>(() =>
        {
            return _singleMember?.Input is not null ?
                Setting()
                    .Title(_singleMember.Format.LocalizeName())
                    .InteractiveElement(_singleMember.Input)
                :
                Card( Stack()
                    .Horizontal()
                    .WithChildren([.._members
                        .SelectMany<TimeSpanInputMember, IUIElement>(
                            i => [
                                i.Input, 
                                Label($"{i.Format}_label", i.Format.LocalizeName() + "        "),
                            ]
                        )
                    ])
                );

        }).Value;

        public TimeSpanInput(params TimeSpanFormat[] formats)
        {
            _members = [..formats.Select(format => {
            var member = new TimeSpanInputMember
            {
                Input = NumberInput(format.ToString(), 0).Step(1).HideCommandBar().Value(0),
                Format = format
            };
            member.Input.OnValueChanged(async value =>
            {
                    if (ReadOnly)
                    {
                        return;
                    }
                    ReadOnly = true;
                    var inputTimeSpan = DoubleToTimeSpan(value, member.Format);
                    var changeTasks = _allTimeSpanInputs.Select(async input =>
                    {
                        input.ReadOnly = true;
                        input.Value = inputTimeSpan;
                        input.ReadOnly = false;
                    });
                    await Task.WhenAll(changeTasks);
                    ReadOnly = false;
             });
            return member;
            })];

            _allTimeSpanInputs.Add(this);
            if (formats.Length == 1)
            {
                _singleMember = _members[0];
            }
        }

        private static TimeSpan DoubleToTimeSpan(double value, TimeSpanFormat format) =>
            format switch
            {
                TimeSpanFormat.Nanoseconds => TimeSpan.FromMicroseconds(value * 1000),
                TimeSpanFormat.Minutes => TimeSpan.FromMinutes(value),
                TimeSpanFormat.Unix => TimeSpan.FromSeconds(Math.Floor(value)),
                TimeSpanFormat.Days => TimeSpan.FromDays(value),
                TimeSpanFormat.Hours => TimeSpan.FromHours(value),
                TimeSpanFormat.Miliseconds => TimeSpan.FromMilliseconds(value),
                TimeSpanFormat.Seconds => TimeSpan.FromSeconds(value),
                TimeSpanFormat.Ticks => TimeSpan.FromTicks(Convert.ToInt64(value)),
                _ => throw new NotImplementedException(),
            };

        private static TimeSpan SetMemberValuesFromTimeStamp(TimeSpan timeSpan, IEnumerable<TimeSpanInputMember> members)
        {
            var result = timeSpan;
            foreach (var member in members)
            {
                var value = member.Format switch
                {
                    TimeSpanFormat.Nanoseconds => timeSpan.Nanoseconds,
                    TimeSpanFormat.Minutes => timeSpan.Minutes,
                    TimeSpanFormat.Unix => timeSpan.Seconds,
                    TimeSpanFormat.Days => timeSpan.Days,
                    TimeSpanFormat.Hours => timeSpan.Hours,
                    TimeSpanFormat.Miliseconds => timeSpan.Milliseconds,
                    TimeSpanFormat.Seconds => timeSpan.Seconds,
                    TimeSpanFormat.Ticks => timeSpan.Ticks,
                    _ => throw new NotImplementedException(),
                };
                result.Subtract(DoubleToTimeSpan(value, member.Format));
                member.Input.Value(value);
            }
            return result;
            
        }

        private static double TimeSpanToDouble(TimeSpan timeSpan, TimeSpanFormat format) =>
            format switch
            {
                TimeSpanFormat.Nanoseconds => timeSpan.TotalNanoseconds,
                TimeSpanFormat.Minutes => timeSpan.TotalMinutes,
                TimeSpanFormat.Unix => Math.Floor(timeSpan.TotalSeconds),
                TimeSpanFormat.Days => timeSpan.TotalDays,
                TimeSpanFormat.Hours => timeSpan.TotalHours,
                TimeSpanFormat.Miliseconds => timeSpan.TotalMilliseconds,
                TimeSpanFormat.Seconds => timeSpan.TotalSeconds,
                TimeSpanFormat.Ticks => timeSpan.Ticks,
                _ => throw new NotImplementedException(),
            };

        private record TimeSpanInputMember
        {
            public required IUINumberInput Input { get; init; }
            public required TimeSpanFormat Format { get; init; }
        }
    }
}