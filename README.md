# RunAsNet 

"RunAsNet" que através de parâmetros de configuração consegue conceder privilégios de outro domínio à uma aplicação qualquer.

É um **"runas /netonly"** melhorado pois a senha é parametrizada.

Para utilizar, no diretório onde se encontra o arquivo **RunAsNet.exe**, crie um arquivo batch com o seguinte conteúdo.

```
runAsNet /user:[DOMINIO\USUARIO] /pass:[SENHA DE REDE] /command:[CAMINHO DA APLICAÇÃO]

```

Podem ser criados quantos batch forem necessários
