import React from 'react';
import './hero-banner.scss';
import classNames from 'classnames';
import { Button } from '@appkit4/react-components';

export interface HeroBannerProps {
    title?: string;
    subTitle?: string;
    bodyText?: string;
    learnMoreButtonText?: string;
    contactUsButtonText?: string;
    onClickLearnMore?: (
        event:
            | React.MouseEvent<HTMLButtonElement>
            | React.KeyboardEvent<HTMLButtonElement>
    ) => void;
    onClickContactUs?: (
        event:
            | React.MouseEvent<HTMLButtonElement>
            | React.KeyboardEvent<HTMLButtonElement>
    ) => void;
    styleType?: 'neutral' | 'primary';
    pattern?: 'left-right' | 'right-left';
    heroImage?: string;
    className?: string;
}

const HeroBanner = React.forwardRef<HTMLElement, HeroBannerProps>(
    (props: HeroBannerProps, ref) => {
        const {
            title = 'Welcome to our service',
            subTitle = 'Transforming your experience',
            bodyText = 'Discover how we can help you achieve your goals with comprehensive solutions tailored to your needs.',
            learnMoreButtonText = 'Learn More',
            contactUsButtonText = 'Contact Us',
            onClickLearnMore,
            onClickContactUs,
            styleType = 'neutral',
            pattern = 'left-right',
            heroImage = require('./svg/discover.svg').default,
            className,
        } = props;

        const heroBannerClass = classNames(
            'ap-pattern-hero-banner',
            className,
            {
                primary: styleType === 'primary',
                neutral: styleType === 'neutral',
                reverse: pattern === 'right-left',
            }
        );

        return (
            <div className={heroBannerClass}>
                <img
                    alt="Hero image, featuring an illustration of red binoculars from the PwC Illustration library, alongside a welcoming message. Below the text are two buttons labeled 'Learn More' and 'Contact Us.'"
                    className="ap-pattern-hero-banner-pic"
                    src={heroImage}
                ></img>
                <div className="ap-pattern-hero-banner-content">
                    <div className="content-title">{title}</div>
                    <div className="content-sub-title">{subTitle}</div>
                    <div className="content-body">{bodyText}</div>
                    <div className="button-container">
                        <Button
                            kind="primary"
                            icon="icon-plus-outline"
                            onClick={onClickLearnMore}
                        >
                            {learnMoreButtonText}
                        </Button>
                        <Button kind="secondary" onClick={onClickContactUs}>
                            {contactUsButtonText}
                        </Button>
                    </div>
                </div>
            </div>
        );
    }
);

export default HeroBanner;
