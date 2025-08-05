# Environment Variables for CI/CD

## Required Secrets

Add these secrets in your GitHub repository settings (Settings > Secrets and variables > Actions):

### Docker Hub (Required for Docker builds)
- `DOCKER_USERNAME` - Your Docker Hub username
- `DOCKER_PASSWORD` - Your Docker Hub password or access token

### SonarCloud (Optional - for code quality analysis)
- `SONAR_TOKEN` - Your SonarCloud token
- Update the organization name in `.github/workflows/code-quality.yml`

### Deployment (Optional - customize based on your deployment target)
- `DEPLOYMENT_TOKEN` - Token for your deployment platform
- `KUBECONFIG` - Kubernetes configuration (if using K8s)
- `AWS_ACCESS_KEY_ID` - AWS access key (if using AWS)
- `AWS_SECRET_ACCESS_KEY` - AWS secret key (if using AWS)

## Environment Configuration

### Staging Environment
- Name: `staging`
- Protection rules: None (auto-deploy from develop branch)

### Production Environment  
- Name: `production`
- Protection rules: Required reviewers, delay timer
- Manual approval required for deployment

## Branch Strategy

- `main` - Production branch (triggers production deployment)
- `develop` - Development branch (triggers staging deployment)
- `feature/*` - Feature branches (triggers CI only)
- `hotfix/*` - Hotfix branches (triggers CI only)

## Workflow Triggers

### CI/CD Pipeline (`ci-cd.yml`)
- Push to `main` or `develop` branches
- Pull requests to `main` branch
- Builds, tests, security scan, Docker build, and deployment

### Release (`release.yml`)
- Git tags starting with `v` (e.g., `v1.0.0`)
- Creates GitHub release with assets
- Publishes Docker image with version tag

### Code Quality (`code-quality.yml`)
- Push to `main` or `develop` branches  
- Pull requests to `main` branch
- SonarCloud analysis and code formatting checks

## Setup Instructions

1. **Enable GitHub Actions** in your repository
2. **Add required secrets** in repository settings
3. **Create environments** for staging and production
4. **Set up branch protection rules** for main branch
5. **Configure SonarCloud project** (optional)
6. **Update deployment scripts** in workflow files for your target platform

## Deployment Customization

Edit the deployment steps in `ci-cd.yml` based on your target platform:

### Kubernetes
```yaml
- name: Deploy to Kubernetes
  run: |
    kubectl set image deployment/productivityapi productivityapi=${{ secrets.DOCKER_USERNAME }}/productivityapi:${{ github.sha }}
```

### Azure Container Instances
```yaml
- name: Deploy to Azure
  run: |
    az container create --resource-group myResourceGroup --name productivityapi --image ${{ secrets.DOCKER_USERNAME }}/productivityapi:${{ github.sha }}
```

### AWS ECS
```yaml
- name: Deploy to AWS ECS
  run: |
    aws ecs update-service --cluster myCluster --service productivityapi --force-new-deployment
```
