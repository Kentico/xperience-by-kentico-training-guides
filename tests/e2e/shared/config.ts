import fs from 'node:fs';
import os from 'node:os';
import path from 'node:path';

const repositoryRoot = path.join(__dirname, '..', '..', '..');

/**
 * Reads the web project's .NET user secrets, so local credentials live in one place with the
 * site's other secrets. Set them with:
 *   dotnet user-secrets set "E2E:AdminPassword" "<password>" --project src/TrainingGuides.Web
 */
function readUserSecrets(): Record<string, unknown> {
  const csproj = fs.readFileSync(path.join(repositoryRoot, 'src', 'TrainingGuides.Web', 'TrainingGuides.Web.csproj'), 'utf8');
  const secretsId = /<UserSecretsId>([^<]+)<\/UserSecretsId>/.exec(csproj)?.[1];
  if (!secretsId) {
    return {};
  }

  const secretsFile = process.platform === 'win32'
    ? path.join(process.env.APPDATA ?? '', 'Microsoft', 'UserSecrets', secretsId, 'secrets.json')
    : path.join(os.homedir(), '.microsoft', 'usersecrets', secretsId, 'secrets.json');

  if (!fs.existsSync(secretsFile)) {
    return {};
  }

  // `dotnet user-secrets set` writes the file with a UTF-8 byte order mark, which JSON.parse rejects.
  return JSON.parse(fs.readFileSync(secretsFile, 'utf8').replace(/^﻿/, ''));
}

const userSecrets = readUserSecrets();

/** `dotnet user-secrets set` stores "E2E:AdminPassword" flat; a hand-edited file may nest it. */
function userSecret(key: string): string | undefined {
  const flat = userSecrets[`E2E:${key}`];
  const nested = (userSecrets.E2E as Record<string, unknown> | undefined)?.[key];
  const value = flat ?? nested;
  return typeof value === 'string' ? value : undefined;
}

/**
 * Every value the tests read, in one place. Environment variables win (that is how CI supplies
 * them), then the web project's user secrets, then a local default. The password has no default.
 */
export const config = {
  baseUrl: process.env.XBK_BASE_URL ?? userSecret('BaseUrl') ?? 'https://localhost:53415',
  adminUsername: process.env.XBK_ADMIN_USERNAME ?? userSecret('AdminUsername') ?? 'administrator',
  adminPassword: process.env.XBK_ADMIN_PASSWORD ?? userSecret('AdminPassword'),
  /** Display name of the website channel the tests create pages in. */
  websiteChannelName: process.env.XBK_WEBSITE_CHANNEL ?? 'Training guides pages',
};

/** Where auth.setup.ts saves the signed-in admin session for the admin tests to reuse. */
export const adminStorageState = path.join(__dirname, '..', '..', 'playwright', '.auth', 'admin.json');
