// The MIT License(MIT)
//
// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

global using System.Collections.Immutable;
global using System.Windows.Input;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Localization;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using Microsoft.UI.Xaml;
global using Microsoft.UI.Xaml.Controls;
global using Microsoft.UI.Xaml.Media;
global using Microsoft.UI.Xaml.Navigation;
global using Refit;
global using Uno.Extensions;
global using Uno.Extensions.Configuration;
global using Uno.Extensions.Hosting;
global using Uno.Extensions.Http;
global using Uno.Extensions.Localization;
global using Uno.Extensions.Logging;
global using Uno.Extensions.Navigation;
global using Uno.Extensions.Reactive;
global using Uno.Extensions.Serialization;
global using UnoPlatformSample.Business.Models;
global using UnoPlatformSample.DataContracts;
global using UnoPlatformSample.DataContracts.Serialization;
global using UnoPlatformSample.Infrastructure;
global using UnoPlatformSample.Presentation;
global using UnoPlatformSample.Services.Caching;
global using UnoPlatformSample.Services.Endpoints;
global using Windows.ApplicationModel;
global using Windows.Networking.Connectivity;
global using Windows.Storage;
global using ApplicationExecutionState = Windows.ApplicationModel.Activation.ApplicationExecutionState;
