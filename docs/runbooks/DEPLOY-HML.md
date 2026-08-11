# Runbook: Implantação em Homologação (HML)

## 1. Pré-Requisitos
- Docker Engine e Docker Compose instalados no servidor de HML ou SDK .NET 10 LTS.
- PostgreSQL 16 acessível.
- Porta HTTP (ex: 8080 ou 5000) liberada no Firewall para a rede do servidor Sytel.

## 2. Passo a Passo de Implantação com Docker
1. Clonar o repositório no servidor de homologação:
   ```bash
   git clone https://github.com/weltonJRC/Projeto_ligo_JRC.git
   cd Jrc.LigoCampaignGateway
   ```
2. Configurar o arquivo `.env` com os valores de HML:
   ```env
   LIGO__MODE=Mock
   LIGO__AUTHBASEURL=https://api.messaging.digitalcontact.cloud
   LIGO__MESSAGINGBASEURL=https://apiwhatsapp.messaging.digitalcontact.cloud
   SYTEL__ALLOWEDTENANT=grupojrc
   SYTEL__ALLOWEDNUMBERCHIP=551148004100
   ```
3. Executar o Docker Compose:
   ```bash
   docker-compose -f deploy/docker/docker-compose.yml up -d --build
   ```
4. Validar o Health Check:
   ```bash
   curl http://localhost:8080/health
   ```

## 3. Configuração do Script Clonado no Sytel
1. Clonar o script `WhatsApp_ativo_manual` no Sytel com o nome `WhatsApp_ativo_manual_MIDIA_HML`.
2. Alterar o `config.xml` do script clonado para apontar para o gateway:
   ```xml
   <config>
     <url>http://10.X.X.X:8080/softdial/Whats_New_API_whatsapp_ativo/SendWhatsAppOutboundTemplate</url>
     <getTemplatesUrl>http://10.X.X.X:8080/softdial/Whats_New_API_whatsapp_ativo/GetWhatsAppOutboundTemplatesCollection</getTemplatesUrl>
     <numberchip>551148004100</numberchip>
     <template>65f9dbce1fb4e9ac773bd386</template>
   </config>
   ```
3. Alterar a requisição no `Start.xml` para incluir o parâmetro de idempotência:
   `&amp;recordId=${cam:id}`
