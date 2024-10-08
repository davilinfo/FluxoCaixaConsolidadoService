# FluxoCaixaConsolidado
Projeto desenvolvido com .NET 7.0 Instruções de como utilizar web api estão disponíveis em Swagger. Se executar a partir de Visual Studio faça em modo administrador para carregar comentários em Swagger.

**Desing Patterns SOLID Principles: baseado em interfaces para serviços e entidades

Microserviço: independente para time, permite escalar horizontalmente e verticalmente

## CQRS: Consultas e persistência em database realizada em repositórios (Persistência), contratos repositórios e entidades em Domínio, Serviços contém regras de negócio e invocação de contratos, controller invocam serviços e fazem validação
## Proxy consulta a serviço externo
## Saga pattern persistência consolidado em outra base de dados

## Annotations: entidades e view models com características de campo e validação

## EntityFramework: EF Core 7, CodeFirst

## AMQP Rabbit MQ: Mensageria pode ser ativada ou desativada em arquivo de appsettings, mas se ativada deve ser instalada versão (3.12.2), em projeto RabbitMQ.Client 6.5.0

## Unit Tests: Utilizado Moq e VisualStudioUnitTests. disponível em Carrefour fluxo caixa

## SQL Server 2016 database criado a partir de migrations (Na pasta da solução executar os seguintes comandos)

dotnet tool install --global dotnet-ef 

dotnet ef database update 20230801051256_FluxoConsolidado --project Persistence -s FluxoCaixaConsolidado -c ConsolidadoContext

## IIS padrão de uso 

## Docker: deve ser realizado publish, gerar imagem e criar container, incluir atributo para permitir consulta a serviços externos --network host,
alterar connectionstring para host.docker.internal,1433
alterar AMQP hostname para host.docker.internal

Para publicar versão da aplicação faça na pasta da solução: Dotnet publish -c Debug

Gerar imagem em docker (na pasta da solução faça): docker build -f ".\FluxoCaixaConsolidado\Dockerfile" --force-rm -t consolidado ".\"

Criar container a partir de imagem: docker run -dt -e "ASPNETCORE_ENVIRONMENT=Development" -e "ASPNETCORE_LOGGING__CONSOLE__DISABLECOLORS=true" -p47155:80 --name consolidado_development consolidado:latest --network host

## RavenDB adicionado (https://ravendb.net/docs/article-page/6.0/csharp/start/getting-started)
Por padrão Raven DB está desativado em appsettings para ativar basta atualizar RavenDB: true
RavenDB configurado pra funcionar em porta 8080
Criar base de dados "FluxoConsolidado" em instância RavenDB (http://127.0.0.1:8080/studio/index.html#databases)
Sync entre sql server e raven acontece ao iniciar API e com ravenDB ativado

Fluxo: ![image](https://github.com/davilinfo/Minsait-FluxoCaixaConsolidadoService/assets/18128361/b469d3d5-9bb6-4b99-8346-4a1bcdf9c14e)

