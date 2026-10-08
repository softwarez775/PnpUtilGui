# PnpUtilGui

Interface gráfica (GUI) moderna para o [pnputil](https://learn.microsoft.com/pt-br/windows-hardware/drivers/devtest/pnputil), o utilitário de linha de comando do Windows para gerenciar drivers.

## Atualizações da versão

### 🎨 Interface
- Está sendo usada a biblioteca [ReaLTaiizor](https://www.nuget.org/packages/ReaLTaiizor), com visual Material Design (tema Indigo no modo claro e Blue Grey no modo escuro).

### ✅ Seleciona todos os itens da lista
- Novo checkbox que marca ou desmarca todos os itens da lista de uma só vez, facilitando exportar ou excluir todos os drivers de uma vez.

### 🌗 Tema automático (claro/escuro)
- O programa detecta automaticamente o tema do Windows e muda a interface dependendo de o sistema estar no tema **light** (claro) ou **dark** (escuro).
- A mudança é aplicada em tempo real: se você trocar o tema do Windows, a interface acompanha sem precisar reiniciar o programa.

### 🌐 Idioma automático
- O programa seleciona automaticamente o idioma de acordo com o idioma configurado no Windows.
- As linguagens disponíveis são: **de, en, fr, pt, ru**.

## Funcionalidades

- **Listar** todos os drivers de terceiros instalados
- **Exportar** drivers selecionados para uma pasta
- **Excluir** drivers selecionados (com opção de forçar exclusão)

## Não suportado

- Importar drivers (planejado)

## Requisitos

- Windows 10/11
- .NET Framework 4.8.1
- Visual Studio (para compilar a partir do código-fonte)

## Como compilar

1. Abra o `PnpUtilGui.sln` no Visual Studio
2. Restaure os pacotes NuGet (botão direito na solução > *Restore NuGet Packages*)
3. Compile com **Ctrl+Shift+B**

> O programa exige execução como administrador para gerenciar drivers.

## Download
 Pronto para usar é só clicar e baixar:
 
Download: ----> [Aqui](https://github.com/softwarez775/PnpUtilGui/releases/download/v1.0.0/PnpUtilGui-v1.0.0.zip)

## Licença

[MIT](LICENSE)
