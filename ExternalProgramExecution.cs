using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools {
	public class ExternalProgramReturnNonzeroException : Exception {
		public int ReturnValue;
		public string Program;
		public string[] Arguments;
		public string StdOut;
		public string StdErr;
		public override string ToString() {
			return Program + " returned " + ReturnValue + ": " + ( StdErr.Trim() != "" ? StdErr : StdOut );
		}
	}

	public class ExternalProgramExecution {
		public static string EscapeArgument( string arg ) {
			// TODO: this isn't enough
			if ( arg.IndexOfAny( new char[] { '"', ' ', '\t', '\\' } ) == -1 ) {
				return "\"" + arg.Replace( "\\", "\\\\" ).Replace( "\"", "\\\"" ) + "\"";
			} else {
				return arg;
			}
		}

		public struct RunProgramReturnValue { public string StdOut; public string StdErr; }
		public static async Task<RunProgramReturnValue> RunProgram( string prog, string[] args, System.Diagnostics.DataReceivedEventHandler[] stdoutCallbacks = null, System.Diagnostics.DataReceivedEventHandler[] stderrCallbacks = null ) {
			return await Task.Run( () => RunProgramSynchronous( prog, args, stdoutCallbacks, stderrCallbacks ) );
		}
		public static RunProgramReturnValue RunProgramSynchronous( string prog, string[] args, System.Diagnostics.DataReceivedEventHandler[] stdoutCallbacks = null, System.Diagnostics.DataReceivedEventHandler[] stderrCallbacks = null ) {
			System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
			startInfo.CreateNoWindow = true;
			startInfo.UseShellExecute = false;
			startInfo.FileName = prog;
			startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

			StringBuilder sb = new StringBuilder();
			foreach ( string s in args ) {
				sb.Append( EscapeArgument( s ) ).Append( " " );
			}
			startInfo.Arguments = sb.ToString();

			startInfo.RedirectStandardOutput = true;
			startInfo.RedirectStandardError = true;
			startInfo.RedirectStandardInput = false;

			using ( System.Diagnostics.Process exeProcess = new System.Diagnostics.Process() ) {
				StringBuilder outputData = new StringBuilder();
				StringBuilder errorData = new StringBuilder();
				exeProcess.OutputDataReceived += ( sender, received ) => {
					if ( !String.IsNullOrEmpty( received.Data ) ) {
						outputData.Append( received.Data );
					}
				};
				if ( stdoutCallbacks != null ) {
					foreach ( System.Diagnostics.DataReceivedEventHandler d in stdoutCallbacks ) {
						exeProcess.OutputDataReceived += d;
					}
				}

				exeProcess.ErrorDataReceived += ( sender, received ) => {
					if ( !String.IsNullOrEmpty( received.Data ) ) {
						errorData.Append( received.Data );
					}
				};
				if ( stderrCallbacks != null ) {
					foreach ( System.Diagnostics.DataReceivedEventHandler d in stderrCallbacks ) {
						exeProcess.ErrorDataReceived += d;
					}
				}

				exeProcess.StartInfo = startInfo;
				exeProcess.Start();
				exeProcess.BeginOutputReadLine();
				exeProcess.BeginErrorReadLine();
				exeProcess.WaitForExit();

				string output = outputData.ToString();
				string err = errorData.ToString();

				if ( exeProcess.ExitCode != 0 ) {
					throw new ExternalProgramReturnNonzeroException() { ReturnValue = exeProcess.ExitCode, Program = prog, Arguments = args, StdOut = output, StdErr = err };
				}

				return new RunProgramReturnValue() { StdOut = output, StdErr = err };
			}
		}
	}
}
