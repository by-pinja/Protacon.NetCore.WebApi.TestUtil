library "jenkins-ptcs-library@4.0.3"

// pod provides common utilies and tools to jenkins-ptcs-library function correctly.
// certain ptcs-library command requires containers (like docker or gcloud.)
podTemplate(label: pod.label,
  containers: pod.templates + [ // This adds all depencies for jenkins-ptcs-library methods to function correctly.
    containerTemplate(name: 'dotnet', image: 'mcr.microsoft.com/dotnet/sdk:6.0', ttyEnabled: true, command: '/bin/sh -c', args: 'cat')
  ]
) {
    node(pod.label) {
      stage('Checkout') {
          checkout scm
      }
      stage('Build') {
        container('dotnet') {
            sh """
                dotnet build
            """
        }
      }
      stage('Test') {
        container('dotnet') {
            sh """
              dotnet test --framework=net6.0 Protacon.NetCore.WebApi.TestUtil.Tests
            """
        }
      }
      stage('Package') {
        container('dotnet') {
          publishTagToNuget("Protacon.NetCore.WebApi.TestUtil")
        }
      }
    }
  }
