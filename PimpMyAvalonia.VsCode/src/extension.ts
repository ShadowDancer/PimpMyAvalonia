import * as path from 'path';
import { workspace, ExtensionContext } from 'vscode';

import {
  LanguageClient,
  LanguageClientOptions,
  ServerOptions,
  TransportKind,
  Executable
} from 'vscode-languageclient';

let client: LanguageClient;

export function activate(context: ExtensionContext) {

  let serverOptions: ServerOptions =  {
    command: 'dotnet',
    args:  ['PimpMyAvalonia.LanguageServer.dll'],
    transport: TransportKind.stdio,
    options: {
      cwd: 'C:\\Users\\przem\\source\\repos\\PimpMyAvalonia\\PimpMyAvalonia.LanguageServer\\bin\\Debug\\netcoreapp3.1\\',
      shell: false
    }
  };

  // Options to control the language client
  let clientOptions: LanguageClientOptions = {
    documentSelector: [
      {
          pattern: "**/*.xaml",
      },
      {
          pattern: "**/*.axaml",
      },
      {
          pattern: "**/*.csproj",
      },
  ],
    synchronize: {
      // Notify the server about file changes to '.clientrc files contained in the workspace
      fileEvents: workspace.createFileSystemWatcher('**/.axaml')
    }
  };

  // Create the language client and start the client.
  client = new LanguageClient(
    'avaloniaxaml',
    'Avalonia Xaml Language Server',
    serverOptions,
    clientOptions
  );

  // Start the client. This will also launch the server
  let disposable = client.start();
  context.subscriptions.push(disposable);
}

export function deactivate(): Thenable<void> | undefined {
  if (!client) {
    return undefined;
  }
  return client.stop();
}
