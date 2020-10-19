using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Keep.Tools.IO
{
  public class CsvWriter : IDisposable
  {
    public class Settings
    {
      public bool EmitHeaders { get; set; }
      public char FieldSeparator { get; set; } = ',';
      public string NullValue { get; set; }
      public string TrueValue { get; set; }
      public string FalseValue { get; set; }
    }

    private readonly TextWriter writer;
    private readonly Settings settings;

    private List<string> headers;
    private List<object> content;

    private bool emitHeader;

    public CsvWriter(TextWriter writer, Settings settings = null)
    {
      this.writer = writer;
      this.settings = settings ?? new Settings();
      this.emitHeader = settings.EmitHeaders;
    }

    public void BeginRecord()
    {
      headers ??= new List<string>();
      content = new List<object>();
    }

    public async Task BeginRecordAsync(CancellationToken stopToken = default)
    {
      BeginRecord();
      await Task.Yield();
    }

    public void WriteField(string name, object value)
    {
      if (!headers.Contains(name))
      {
        headers.Add(name);
      }
      while (content.Count < headers.Count)
      {
        content.Add(null);
      }

      var index = headers.IndexOf(name);
      content[index] = value;
    }

    public async Task WriteFieldAsync(string name, object value,
      CancellationToken stopToken = default)
    {
      WriteField(name, value);
      await Task.Yield();
    }

    public void EndRecord()
    {
      EndRecordAsync().Await();
    }

    public async Task EndRecordAsync(CancellationToken stopToken = default)
    {
      if (emitHeader)
      {
        await WriteRecordAsync(headers, stopToken);
        emitHeader = false;
      }
      await WriteRecordAsync(content, stopToken);
    }

    private async Task WriteRecordAsync(ICollection values,
      CancellationToken stopToken)
    {
      int count = 0;
      foreach (object value in values)
      {
        if (stopToken.IsCancellationRequested)
          return;

        if (++count > 1)
        {
          await writer.WriteAsync(settings.FieldSeparator);
        }

        await writer.WriteAsync(CreateString(value));
      }
    }

    private string CreateString(object value)
    {
      if (value == null || value == DBNull.Value)
        return settings.NullValue ?? "(null)";

      if (value is bool bit)
        return bit
          ? settings.TrueValue ?? Change.To<string>(true)
          : settings.TrueValue ?? Change.To<string>(false);

      return Change.To<string>(value).Trim();
    }

    public void Flush()
    {
      writer.Flush();
    }

    public async Task FlushAsync(CancellationToken stopToken = default)
    {
      await writer.FlushAsync();
    }

    public void Dispose()
    {
      Flush();
    }
  }
}
