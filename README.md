# FluxoCaixaConsolidado
Projeto desenvolvido com .NET 7.0 Instruções de como utilizar web api estão disponíveis em Swagger

**Desing Patterns SOLID Principles: baseado em interfaces para serviços e entidades

Microserviço: independente para time, permite escalar horizontalmente e verticalmente

##CQRS (Não inteiramente já que não existe uma outra base de dados para read na solução): Consultas e persistência em database realizada em repositórios (Persistência), contratos repositórios e entidades em Domínio, Serviços contém regras de negócio e invocação de contratos, controller invocam serviços e fazem validação
##Proxy consulta a serviço externo
##Saga pattern persistência consolidado em outra base de dados

##Annotations: entidades e view models com características de campo e validação

##EntityFramework: EF Core 7, CodeFirst

##AMQP Rabbit MQ: Mensageria pode ser ativada ou desativada em arquivo de appsettings, mas se ativada deve ser instalada versão (3.12.2), em projeto RabbitMQ.Client 6.5.0

##Unit Tests: Utilizado Moq e VisualStudioUnitTests. disponível em Carrefour fluxo caixa

##SQL Server 2016 gerado a partir de migrations ##IIS padrão de uso 

##Docker: deve ser realizado publish, gerar imagem e criar container, incluir atributo para permitir consulta a serviços externos --network host,
alterar connectionstring para host.docker.interval,1433
alterar AMQP hostname para host.docker.interval

Para publicar versão da aplicação faça na pasta da solução: Dotnet publish -c Debug

Gerar imagem em docker: docker build -f "D:\RestfulApi\FluxoCaixaConsolidado\FluxoCaixaConsolidado\Dockerfile" --force-rm -t consolidado "D:\RestfulApi\FluxoCaixaConsolidado"

Criar container a partir de imagem: docker run -dt -e "ASPNETCORE_ENVIRONMENT=Development" -e "ASPNETCORE_LOGGING__CONSOLE__DISABLECOLORS=true" -p47155:80 --name consolidado_development consolidado:latest --network host
