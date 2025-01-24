using System;
using Cysharp.Threading.Tasks;

namespace Domains.Utils
{
	public interface IStatusReporter
	{
		void ReportStatus();
	}
}