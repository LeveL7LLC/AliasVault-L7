#!/bin/sh

# Create SSL directory if it doesn't exist
mkdir -p /app/ssl

# Generate self-signed SSL certificate if not exists
if [ ! -f /app/ssl/api.crt ] || [ ! -f /app/ssl/api.key ]; then
    echo "Generating new SSL certificate..."
    openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
        -keyout /app/ssl/api.key \
        -out /app/ssl/api.crt \
        -subj "/C=US/ST=State/L=City/O=Organization/CN=localhost"

    # Set proper permissions
    chmod 644 /app/ssl/api.crt
    chmod 600 /app/ssl/api.key

    # Create PFX for ASP.NET Core
    openssl pkcs12 -export -out /app/ssl/api.pfx \
        -inkey /app/ssl/api.key \
        -in /app/ssl/api.crt \
        -password pass:YourSecurePassword
fi

export ASPNETCORE_Kestrel__Certificates__Default__Path=/app/ssl/api.pfx
export ASPNETCORE_Kestrel__Certificates__Default__Password=YourSecurePassword

# Start the application
dotnet AliasVault.Api.dll
